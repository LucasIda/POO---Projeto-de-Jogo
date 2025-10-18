using Godot;
using System;
using System.Collections.Generic;

public partial class JokerCard : BaseCard
{
    private List<IJokerEffect> _effects = new();

    public void AddEffect(IJokerEffect effect)
    {
        _effects.Add(effect);
    }

    public void ActivateEffects(HandValue.HandResult result)
    {
        foreach (var effect in _effects)
            effect.Apply(result);
    }

    public override string ToString()
    {
        return $"Joker: {Name} ({_effects.Count} effects)";
    }
}
