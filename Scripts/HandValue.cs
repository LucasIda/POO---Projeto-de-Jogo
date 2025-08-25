using Godot;
using System;
using System.Collections.Generic;

/// Calcula a pontuação da mão baseada no tipo de mão do jogador.
/// Usa o script HandChecker para referência das mãos.
public static class HandValue
{
    // Classe interna para armazenar Chips e Multiplicador
    private class HandScoreData
    {
        public int Chips { get; }
        public int Multiplier { get; }

        public HandScoreData(int chips, int multiplier)
        {
            Chips = chips;
            Multiplier = multiplier;
        }

        public int GetScore() => Chips * Multiplier;
    }

    private static readonly Dictionary<HandChecker.HandType, HandScoreData> HandTable = new()
    {
        { HandChecker.HandType.HighCard,      new HandScoreData(5, 1)   },
        { HandChecker.HandType.Pair,          new HandScoreData(10, 2)  },
        { HandChecker.HandType.TwoPair,       new HandScoreData(15, 3)  },
        { HandChecker.HandType.ThreeOfAKind,  new HandScoreData(20, 5)  },
        { HandChecker.HandType.Straight,      new HandScoreData(25, 8)  },
        { HandChecker.HandType.Flush,         new HandScoreData(30, 10) },
        { HandChecker.HandType.FullHouse,     new HandScoreData(40, 15) },
        { HandChecker.HandType.FourOfAKind,   new HandScoreData(50, 20) },
        { HandChecker.HandType.FiveOfAKind,   new HandScoreData(60, 25) },
        { HandChecker.HandType.StraightFlush, new HandScoreData(75, 50) },
        { HandChecker.HandType.RoyalFlush,    new HandScoreData(100, 100)},
        { HandChecker.HandType.FlushHouse,    new HandScoreData(80, 40) },
        { HandChecker.HandType.FlushFive,     new HandScoreData(90, 45) }
    };

    public static int GetChips(HandChecker.HandType hand)
    {
        return hand switch
        {
            HandChecker.HandType.HighCard => 1,
            HandChecker.HandType.Pair => 2,
            HandChecker.HandType.TwoPair => 3,
            HandChecker.HandType.ThreeOfAKind => 4,
            HandChecker.HandType.Straight => 5,
            HandChecker.HandType.Flush => 6,
            HandChecker.HandType.FullHouse => 7,
            HandChecker.HandType.FourOfAKind => 8,
            HandChecker.HandType.FiveOfAKind => 9,
            HandChecker.HandType.StraightFlush => 10,
            HandChecker.HandType.RoyalFlush => 11,
            HandChecker.HandType.FlushHouse => 12,
            HandChecker.HandType.FlushFive => 13,
            _ => 0
        };
    }

    public static int GetMultiplier(HandChecker.HandType hand)
    {
        return hand switch
        {
            HandChecker.HandType.HighCard => 1,
            HandChecker.HandType.Pair => 1,
            HandChecker.HandType.TwoPair => 2,
            HandChecker.HandType.ThreeOfAKind => 2,
            HandChecker.HandType.Straight => 3,
            HandChecker.HandType.Flush => 3,
            HandChecker.HandType.FullHouse => 4,
            HandChecker.HandType.FourOfAKind => 5,
            HandChecker.HandType.FiveOfAKind => 6,
            HandChecker.HandType.StraightFlush => 7,
            HandChecker.HandType.RoyalFlush => 10,
            HandChecker.HandType.FlushHouse => 8,
            HandChecker.HandType.FlushFive => 9,
            _ => 1
        };
    }

    public static int GetScore(HandChecker.HandType hand)
    {
        int chips = GetChips(hand);
        int mult = GetMultiplier(hand);
        return chips * mult;
    }
}
