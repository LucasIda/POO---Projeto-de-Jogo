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
        { HandChecker.HandType.RoyalFlush,    (120,10) },
        { HandChecker.HandType.FlushHouse,    (80, 5)  },
        { HandChecker.HandType.FiveOfAKind,   (100,6)  },
        { HandChecker.HandType.FlushFive,     (150,8)  }
    };

    // Agora com curingas opcionais
    public static HandResult Evaluate(HandChecker.HandType handType, List<CardData> cards, List<JokerCard> jokers = null)
    {
        var (baseChips, mult) = HandTable.TryGetValue(handType, out var data) ? data : (0, 1);

        // Soma o ChipValue das cartas relevantes
        var relevantCards = GetRelevantCards(handType, cards);
        int chipsFromCards = relevantCards.Sum(c => c.ChipValue);

        int totalChips = baseChips + chipsFromCards;

        int totalMultiplier = mult;

        // Aplica efeito dos curingas
        if (jokers != null && jokers.Count > 0)
        {
            foreach (var joker in jokers)
                totalMultiplier *= joker.Multiplier;
        }

        return new HandResult(totalChips, totalMultiplier);
    }

    private static List<CardData> GetRelevantCards(HandChecker.HandType handType, List<CardData> cards)
    {
        switch (handType)
        {
            case HandChecker.HandType.HighCard:
                return cards.OrderByDescending(c => (int)c.Rank).Take(1).ToList();
            case HandChecker.HandType.Pair:
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
            case HandChecker.HandType.FourOfAKind:
                return cards.GroupBy(c => c.Rank)
                            .Where(g => g.Count() == 4)
                            .SelectMany(g => g)
                            .ToList();
            case HandChecker.HandType.FullHouse:
                return cards.GroupBy(c => c.Rank)
                            .Where(g => g.Count() >= 2)
                            .SelectMany(g => g)
                            .ToList();
            case HandChecker.HandType.Straight:
            case HandChecker.HandType.Flush:
            case HandChecker.HandType.StraightFlush:
            case HandChecker.HandType.RoyalFlush:
            case HandChecker.HandType.FlushHouse:
            case HandChecker.HandType.FiveOfAKind:
            case HandChecker.HandType.FlushFive:
                return cards;
            default:
                return new List<CardData>();
        }
    }
}
