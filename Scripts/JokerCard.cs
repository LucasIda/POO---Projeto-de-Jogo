using Godot;
using System;
using System.Collections.Generic;

public enum TooltipDirection
{
    Above,
    Below
}

public enum JokerRarity
{
    Common,
    Uncommon,
    Rare,
    Legendary
}

public partial class JokerCard : BaseCard
{
    private List<IJokerEffect> _effects = new();
    public int Cost { get; private set; }

    private JokerRarity _rarity;
    private bool _rarityInitialized = false;
    public JokerRarity Rarity
    {
        get => _rarity;
        set
        {
            _rarity = value;
            _rarityInitialized = true;  // Marca como inicializado
            Cost = CalculateCostFromRarity(_rarity);
        }
    }

    

    private PanelContainer tooltip;

    public TooltipDirection TooltipDisplayDirection { get; set; } = TooltipDirection.Above;

    public void AddEffect(IJokerEffect effect)
    {
        _effects.Add(effect);
    }

    public void ActivateEffects(HandValue.HandResult result, List<CardData> playedCards)

    {
        foreach (var effect in _effects)
            effect.Apply(result, playedCards);
    }

    public override string ToString()
    {
        return $"Joker: {Name} ({_effects.Count} effects)";
    }

    private void OnMouseEntered()
    {
        if (IsDragging) return;
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
            Modulate = new Color(255, 255, 255, 1), 
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

        var nameLabel = new Label { Text = $"Nome: {Name}" };
        nameLabel.Modulate = Colors.Black;
        vbox.AddChild(nameLabel);

        var rarityLabel = new Label { Text = $"Raridade: {Rarity}" };
        rarityLabel.Modulate = Colors.Black;
        vbox.AddChild(rarityLabel);

        var costLabel = new Label { Text = $"Custo: {Cost} moedas" };
        costLabel.Modulate = Colors.Black;
        vbox.AddChild(costLabel);
        
        foreach(var effect in _effects)
        {
            var effectLabel = new Label { Text = $"Efeito: {effect.Description}" };
            effectLabel.Modulate = Colors.Black;
            vbox.AddChild(effectLabel);
        }

        tooltip.AddChild(vbox);
        AddChild(tooltip);

        Vector2 position;
        if (TooltipDisplayDirection == TooltipDirection.Below)
        {
            position = new Vector2(0, Size.Y + 10);
        }
        else
        {
            position = new Vector2(0, -tooltip.Size.Y - 10);
        }
        tooltip.CallDeferred("set_position", position);
    }

    protected override void HideTooltip()
{
    base.HideTooltip();
    if (tooltip != null)
    {
        tooltip.QueueFree();
        tooltip = null;
    }
}

    public override void _Ready()
    {
        base._Ready();
        MouseEntered += OnMouseEntered;
        MouseExited += OnMouseExited;

        if (!_rarityInitialized)
        {
            Rarity = JokerRarity.Common;
        }
    }

    private int CalculateCostFromRarity(JokerRarity rarity)
    {
        switch (rarity)
        {
            case JokerRarity.Common:
                return 7;
            case JokerRarity.Uncommon:
                return 9;
            case JokerRarity.Rare:
                return 11;
            case JokerRarity.Legendary:
                return 35;
            default:
                GD.PrintErr($"Raridade desconhecida: {rarity}. Usando custo padrão (5).");
                return 5; 
        }
    }
}