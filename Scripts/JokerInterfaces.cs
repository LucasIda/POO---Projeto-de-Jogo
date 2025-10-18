using Godot;

public interface IJokerEffect
{
    void Apply(HandValue.HandResult result);
}

public class EffectAddChips : IJokerEffect
{
    private int _chips;

    public EffectAddChips(int chips)
    {
        _chips = chips;
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

    public EffectAddMultiplier(int multiplier)
    {
        _multiplier = multiplier;
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

    public EffectMultiplyMultiplier(float factor)
    {
        _factor = factor;
    }

    public void Apply(HandValue.HandResult result)
    {
        result.MultBase = (int)(result.MultBase * _factor);
        GD.Print($"🔴 Multiplier multiplied by {_factor}");
    }
}

