using Godot;
using System;

public partial class Card : TextureRect
{
    public CardData Data { get; private set; }
    public bool IsSelected { get; private set; }
    private bool _isDragging;
    public bool IsDragging => _isDragging;

    public delegate void CardClicked(Card clickedCard);
    public event CardClicked OnCardClicked;

    public delegate void CardDrag(Card card, Vector2 delta);
    public event CardDrag OnDragging;

    public delegate void CardDragEnd(Card card);
    public event CardDragEnd OnDragEnded;

    private Vector2 _dragOffset;
    private PanelContainer tooltip;

    public void SetCard(CardData data, Texture2D texture)
    {
        Data = data;
        Texture = texture;

        // Conectar eventos de mouse
        Connect("gui_input", new Callable(this, nameof(OnCardInput)));
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }

    private void OnCardInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent)
        {
            if (mouseEvent.ButtonIndex == MouseButton.Left)
            {
                if (mouseEvent.Pressed)
                {
                    OnCardClicked?.Invoke(this);
                    _dragOffset = GetGlobalMousePosition() - GlobalPosition;
                    _isDragging = true;
                }
                else
                {
                    _isDragging = false;
                    OnDragEnded?.Invoke(this);
                }
            }
        }
        else if (@event is InputEventMouseMotion motion && _isDragging)
        {
            GlobalPosition = GetGlobalMousePosition() - _dragOffset;
            OnDragging?.Invoke(this, motion.Relative);
        }
    }

    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
        Modulate = IsSelected
            ? new Color(1, 1, 1, 0.5f)
            : new Color(1, 1, 1, 1);
    }

    private void OnMouseEntered()
    {
        ShowTooltip();
    }

    private void OnMouseExited()
    {
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (tooltip != null) return;

        tooltip = new PanelContainer
        {
            Modulate = new Color(0, 0, 0, 0.85f),
            SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
            SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
        };

        tooltip.AddThemeConstantOverride("margin_left", 6);
        tooltip.AddThemeConstantOverride("margin_right", 6);
        tooltip.AddThemeConstantOverride("margin_top", 4);
        tooltip.AddThemeConstantOverride("margin_bottom", 4);

        var vbox = new VBoxContainer
        {
            CustomMinimumSize = new Vector2(120, 0)
        };
        vbox.AddThemeConstantOverride("separation", 4);

        var nameLabel = new Label { Text = $"Nome: {Data.Name}" };
        var chipLabel = new Label { Text = $"Chips: {GetChipValue()}" };

        vbox.AddChild(nameLabel);
        vbox.AddChild(chipLabel);

        tooltip.AddChild(vbox);
        AddChild(tooltip);
        tooltip.Position = new Vector2(0, -tooltip.Size.Y - 10);
    }

    private void HideTooltip()
    {
        tooltip?.QueueFree();
        tooltip = null;
    }

    private int GetChipValue()
    {
        return Data.Rank switch
        {
            Rank.Ace => 11,
            Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => 10,
            _ => (int)Data.Rank
        };
    }
}
