using Godot;
using System;

public abstract partial class BaseCard : TextureRect
{
    public string Name { get; protected set; }
    public Texture2D TextureIcon { get; protected set; }

    public bool IsSelected { get; private set; }
    private bool _isDragging;
    public bool IsDragging => _isDragging;
    public bool IsDraggable { get; set; } = true;

    public delegate void CardClicked(BaseCard clickedCard);
    public event CardClicked OnCardClicked;

    public delegate void CardDrag(BaseCard card, Vector2 delta);
    public event CardDrag OnDragging;

    public delegate void CardDragEnd(BaseCard card);
    public event CardDragEnd OnDragEnded;

    private Vector2 _dragOffset;

    public virtual void Initialize(string name, Texture2D texture)
    {
        Name = name;
        TextureIcon = texture;
        Texture = texture;

        Connect("gui_input", new Callable(this, nameof(OnCardInput)));
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

                    if (!IsDraggable)
                    {
                        return;
                    }

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
        Modulate = IsSelected ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);
    }
}
