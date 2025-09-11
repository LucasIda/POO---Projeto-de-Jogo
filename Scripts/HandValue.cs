using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class HandValue
{
    public class HandResult
    {
        public int Chips { get; }
        public int Multiplier { get; }
        public int Score => Chips * Multiplier;

        public HandResult(int chips, int multiplier)
        {
            Chips = chips;
            Multiplier = multiplier;
        }
    }

    private static readonly Dictionary<HandChecker.HandType, (int Chips, int Mult)> HandTable = new()
    {
        { HandChecker.HandType.HighCard,      (5, 1)   },  
        { HandChecker.HandType.Pair,          (10, 2)  },  
        { HandChecker.HandType.TwoPair,       (20, 2)  },  
        { HandChecker.HandType.ThreeOfAKind,  (30, 3)  },  
        { HandChecker.HandType.Straight,      (30, 4)  },  
        { HandChecker.HandType.Flush,         (35, 4)  },  
        { HandChecker.HandType.FullHouse,     (40, 4)  },  
        { HandChecker.HandType.FourOfAKind,   (60, 7)  },  
        { HandChecker.HandType.StraightFlush, (100, 8) },  
        { HandChecker.HandType.RoyalFlush,    (120, 10)}  
    };

    /// Avalia a mão e retorna chips, multiplicador e score.
    public static HandResult Evaluate(HandChecker.HandType handType, List<CardData> cards)
    {
        var (baseChips, mult) = HandTable.TryGetValue(handType, out var data) ? data : (0, 1);

        //pega só as cartas relevantes para chips
        var relevantCards = GetRelevantCards(handType, cards);

        int chipsFromCards = relevantCards.Sum(c => c.ChipValue);

        int totalChips = chipsFromCards + baseChips;

        return new HandResult(totalChips, mult);
    }

    /// Retorna apenas as cartas relevantes para contagem de chips
    private static List<CardData> GetRelevantCards(HandChecker.HandType handType, List<CardData> cards)
    {
        switch (handType)
        {
            case HandChecker.HandType.HighCard:
                // só a carta mais alta
                return cards.OrderByDescending(c => (int)c.Rank).Take(1).ToList();

            case HandChecker.HandType.Pair:
                return cards.GroupBy(c => c.Rank)
                            .Where(g => g.Count() == 2)
                            .SelectMany(g => g)
                            .ToList();

            case HandChecker.HandType.TwoPair:
                return cards.GroupBy(c => c.Rank)
                            .Where(g => g.Count() == 2)
                            .SelectMany(g => g)
                            .ToList();

            case HandChecker.HandType.ThreeOfAKind:
                return cards.GroupBy(c => c.Rank)
                            .Where(g => g.Count() == 3)
                            .SelectMany(g => g)
                            .ToList();

            case HandChecker.HandType.Straight:
            case HandChecker.HandType.Flush:
            case HandChecker.HandType.StraightFlush:
            case HandChecker.HandType.RoyalFlush:
                // todas as cartas da mão
                return cards;

            case HandChecker.HandType.FullHouse:
                return cards.GroupBy(c => c.Rank)
                            .Where(g => g.Count() >= 2) // pega trio e par
                            .SelectMany(g => g)
                            .ToList();

            case HandChecker.HandType.FourOfAKind:
                return cards.GroupBy(c => c.Rank)
                            .Where(g => g.Count() == 4)
                            .SelectMany(g => g)
                            .ToList();

            default:
                return new List<CardData>();
        }
    }
}
