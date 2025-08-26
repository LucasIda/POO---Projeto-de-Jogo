using Godot;
using System;
using System.Collections.Generic;

public static class HandValue
{
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

    // Tabela de valores para cada tipo de mão
    private static readonly Dictionary<HandChecker.HandType, HandScoreData> HandTable = new()
    {
        { HandChecker.HandType.HighCard,      new HandScoreData(5, 1)   },  
        { HandChecker.HandType.Pair,          new HandScoreData(10, 2)  },  
        { HandChecker.HandType.TwoPair,       new HandScoreData(20, 2)  },  
        { HandChecker.HandType.ThreeOfAKind,  new HandScoreData(30, 3)  },  
        { HandChecker.HandType.Straight,      new HandScoreData(30, 4)  },  
        { HandChecker.HandType.Flush,         new HandScoreData(35, 4)  },  
        { HandChecker.HandType.FullHouse,     new HandScoreData(40, 4)  },  
        { HandChecker.HandType.FourOfAKind,   new HandScoreData(60, 7)  },  
        { HandChecker.HandType.StraightFlush, new HandScoreData(100, 8) },  
        { HandChecker.HandType.RoyalFlush,    new HandScoreData(120, 10)}  
    };

    // Retorna fichas (Chips)
    public static int GetChips(HandChecker.HandType handType)
    {
        return HandTable.TryGetValue(handType, out var data) ? data.Chips : 0;
    }

    // Retorna multiplicador (Mult)
    public static int GetMultiplier(HandChecker.HandType handType)
    {
        return HandTable.TryGetValue(handType, out var data) ? data.Multiplier : 0;
    }

    // Retorna pontuação total (Chips * Mult)
    public static int GetScore(HandChecker.HandType handType)
    {
        return HandTable.TryGetValue(handType, out var data) ? data.GetScore() : 0;
    }
}
