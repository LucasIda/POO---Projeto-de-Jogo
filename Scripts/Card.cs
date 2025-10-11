using Godot;
using System;

public partial class Card : BaseCard
{
    public CardData Data { get; private set; }

    private PanelContainer tooltip;

    public void SetCard(CardData data, Texture2D texture)
    {
        Data = data;
        Initialize(data.Name, texture);
    }

    public int GetChipValue()
    {
        return Data.Rank switch
        {
            Rank.Ace => 11,
            Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => 10,
            _ => (int)Data.Rank
        };
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

        // Caixa branca
        tooltip = new PanelContainer
        {
            Modulate = new Color(255, 255, 255, 1), // branco sólido
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

        // Letras pretas usando Modulate
        var nameLabel = new Label { Text = $"Nome: {Data.Name}" };
        nameLabel.Modulate = Colors.Black;

        var chipLabel = new Label { Text = $"Chips: {GetChipValue()}" };
        chipLabel.Modulate = Colors.Black;

        vbox.AddChild(nameLabel);
        vbox.AddChild(chipLabel);

        tooltip.AddChild(vbox);
        AddChild(tooltip);

        tooltip.CallDeferred("set_position", new Vector2(0, -tooltip.Size.Y - 10));
    }



    private void HideTooltip()
    {
        tooltip?.QueueFree();
        tooltip = null;
    }

    public override void _Ready()
    {
        base._Ready();
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;
    }
}
