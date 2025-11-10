using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public interface IJokerEffect
{
    string Description { get; }
    void Apply(HandValue.HandResult result, List<CardData> playedCards);
}

public interface ICardFilter
{
    string DescriptionFragment { get; }
    int Count(List<CardData> playedCards);
}

public class FilterBySuit : ICardFilter
{
    private readonly Suit _targetSuit;
    public string DescriptionFragment => $"carta de {_targetSuit}";

    public FilterBySuit(Suit targetSuit)
    {
        _targetSuit = targetSuit;
    }

    public int Count(List<CardData> playedCards)
    {
        return playedCards.Count(c => c.Suit == _targetSuit);
    }
}
public class FilterByRank : ICardFilter
{
    private readonly Rank _targetRank;
    public string DescriptionFragment => $"carta de rank {_targetRank}"; 

    public FilterByRank(Rank targetRank)
    {
        _targetRank = targetRank;
    }
    public int Count(List<CardData> playedCards)
    {
        return playedCards.Count(c => c.Rank == _targetRank);
    }
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

    public void Apply(HandValue.HandResult result, List<CardData> playedCards)
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

    public void Apply(HandValue.HandResult result, List<CardData> playedCards)
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

    public void Apply(HandValue.HandResult result, List<CardData> playedCards)
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

    public void Apply(HandValue.HandResult result, List<CardData> playedCards)
    {
        int playerCoins = _getPlayerCoins();
    int additionalChips = playerCoins * _multiplierPerCoin;
    result.ChipsBase += additionalChips;

        GD.Print($"+{additionalChips} chips (PlayerCoins={playerCoins}, x{_multiplierPerCoin})");
    }
}
public class EffectAddMultPerFilteredCard : IJokerEffect
{
    private readonly int _multPerCard;
    private readonly ICardFilter _filter;
    public string Description { get; }

    public EffectAddMultPerFilteredCard(int multPerCard, ICardFilter filter, string description)
    {
        _multPerCard = multPerCard;
        _filter = filter;
        Description = description;
    }

    public void Apply(HandValue.HandResult result, List<CardData> playedCards)
    {
        int cardCount = _filter.Count(playedCards);

        if (cardCount > 0)
        {
            int totalMult = cardCount * _multPerCard;
            result.MultBase += totalMult;
            GD.Print($"🔷 +{totalMult} Mult ({cardCount}x {_filter.DescriptionFragment})");
        }
    }

}
public class EffectMultiplyMultiplierPerFilteredCard : IJokerEffect
{
    private readonly float _factor; // O 1.5f
    private readonly ICardFilter _filter;
    public string Description { get; }

    public EffectMultiplyMultiplierPerFilteredCard(float factor, ICardFilter filter, string description)
    {
        _factor = factor;
        _filter = filter;
        Description = description;
    }

    public void Apply(HandValue.HandResult result, List<CardData> playedCards)
    {
        int cardCount = _filter.Count(playedCards);
        if (cardCount > 0)
        {
            double totalMultiplier = Math.Pow(_factor, cardCount);
            result.MultBase = (int)(result.MultBase * totalMultiplier);
            GD.Print($"🔴 Mult x{totalMultiplier:F2}! ({_factor}x^{cardCount} por {_filter.DescriptionFragment})");
        }
    }
}

