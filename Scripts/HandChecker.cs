using System;
using System.Collections.Generic;
using System.Linq;

public static class HandChecker
{
    public enum HandType
    {
        HighCard,
        Pair,
        TwoPair,
        ThreeOfAKind,
        Straight,
        Flush,
        FullHouse,
        FourOfAKind,
        FiveOfAKind,
        StraightFlush,
        RoyalFlush,
        FlushHouse,
        FlushFive
    }

    public static HandType EvaluateHand(List<CardData> selectedCards)
    {
        if (selectedCards == null || selectedCards.Count == 0)
            return HandType.HighCard;

        var ranks = selectedCards.Select(c => c.Rank).ToList();
        var uniqueRanks = ranks.Distinct().ToList();

        bool hasPair = ranks.GroupBy(r => r).Any(g => g.Count() == 2);
        bool hasTwoPair = ranks.GroupBy(r => r).Count(g => g.Count() == 2) >= 2;
        bool hasThreeOfAKind = ranks.GroupBy(r => r).Any(g => g.Count() == 3);
        bool hasFourOfAKind = ranks.GroupBy(r => r).Any(g => g.Count() == 4);
        bool hasFiveOfAKind = ranks.GroupBy(r => r).Any(g => g.Count() == 5);

        bool isFlush = selectedCards.GroupBy(c => c.Suit)
                            .Any(g => g.Count() >= 5);
        bool isStraight = CheckStraight(uniqueRanks);
        bool isRoyal = uniqueRanks.Contains(Rank.Ten) &&
                       uniqueRanks.Contains(Rank.Jack) &&
                       uniqueRanks.Contains(Rank.Queen) &&
                       uniqueRanks.Contains(Rank.King) &&
                       uniqueRanks.Contains(Rank.Ace);

        // Prioridade das mãos: mais forte primeiro
        if (isFlush && hasFiveOfAKind) return HandType.FlushFive;
        if (isFlush && hasThreeOfAKind && hasPair) return HandType.FlushHouse;
        if (isFlush && isRoyal) return HandType.RoyalFlush;
        if (isFlush && isStraight) return HandType.StraightFlush;
        if (hasFiveOfAKind) return HandType.FiveOfAKind;
        if (hasFourOfAKind) return HandType.FourOfAKind;
        if (hasThreeOfAKind && hasPair) return HandType.FullHouse;
        if (isFlush) return HandType.Flush;
        if (isStraight) return HandType.Straight;
        if (hasThreeOfAKind) return HandType.ThreeOfAKind;
        if (hasTwoPair) return HandType.TwoPair;
        if (hasPair) return HandType.Pair;

        return HandType.HighCard;
    }

    private static bool CheckStraight(List<Rank> uniqueRanks)
    {
        var ordered = uniqueRanks.OrderBy(r => (int)r).ToList();
        int consecutive = 1;

        for (int i = 1; i < ordered.Count; i++)
        {
            if ((int)ordered[i] == (int)ordered[i - 1] + 1)
            {
                consecutive++;
                if (consecutive >= 5) return true;
            }
            else
            {
                consecutive = 1;
            }
        }

        // Tratamento especial Ás baixo (A-2-3-4-5)
        if (uniqueRanks.Contains(Rank.Ace))
        {
            var lowAceOrdered = ordered.Select(r => r == Rank.Ace ? 1 : (int)r).OrderBy(r => r).ToList();
            consecutive = 1;
            for (int i = 1; i < lowAceOrdered.Count; i++)
            {
                if (lowAceOrdered[i] == lowAceOrdered[i - 1] + 1)
                {
                    consecutive++;
                    if (consecutive >= 5) return true;
                }
                else consecutive = 1;
            }
        }

        return false;
    }
}
