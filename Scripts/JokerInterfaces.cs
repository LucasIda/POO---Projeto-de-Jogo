using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IJokerEffect
{
    string Description { get; }
    void Apply(HandValue.HandResult result);
}

public class EffectAddChips : IJokerEffect
{
    private int _chips;
    public string Description { get; }

    public EffectAddChips(int chips, string description)
    {
        _chips = chips;
        Description = description;
    }

    public void Apply(HandValue.HandResult result)
    {
        result.ChipsBase += _chips;
        GD.Print($"🟢 +{_chips} chips added by Joker");
    }
}


public class EffectAddMultiplier : IJokerEffect
{
    private int _multiplier;
    public string Description { get; }

    public EffectAddMultiplier(int multiplier, string description)
    {
        _multiplier = multiplier;
        Description = description;
    }

    public void Apply(HandValue.HandResult result)
    {
        result.MultBase += _multiplier;
        GD.Print($"🔵 +{_multiplier}x multiplier added by Joker");
    }
}


public class EffectMultiplyMultiplier : IJokerEffect
{
    private float _factor;
    public string Description { get; }

    public EffectMultiplyMultiplier(float factor, string description)
    {
        _factor = factor;
        Description = description;
    }

    public void Apply(HandValue.HandResult result)
    {
        result.MultBase = (int)(result.MultBase * _factor);
        GD.Print($"🔴 Multiplier multiplied by {_factor}");
    }
}


public class EffectAddChipsPerCoin : IJokerEffect
{
    private readonly int _multiplierPerCoin;
    private readonly Func<int> _getPlayerCoins;
    public string Description { get; }

    public EffectAddChipsPerCoin(int multiplierPerCoin, Func<int> getPlayerCoins, string description)
    {
        _multiplierPerCoin = multiplierPerCoin;
        _getPlayerCoins = getPlayerCoins;
        Description = description;
    }

    public void Apply(HandValue.HandResult result)
    {
        int playerCoins = _getPlayerCoins();
        int additionalChips = playerCoins * _multiplierPerCoin;
        result.ChipsBase += additionalChips;

        GD.Print($"+{additionalChips} chips (PlayerCoins={playerCoins}, x{_multiplierPerCoin})");
    }
}