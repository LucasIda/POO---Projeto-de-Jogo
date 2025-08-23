using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public static class HandEvaluator
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

        bool isFlush = false;
        bool isStraight = false;
        bool hasPair = false;
        bool hasTwoPair = false;
        bool hasThreeOfAKind = false;
        bool hasFourOfAKind = false;
        bool hasFiveOfAKind = false;
        bool isRoyal = false;

        // ====== Contar ranks ======
        var ranks = selectedCards.Select(c => c.Rank).ToList();
        var uniqueRanks = ranks.Distinct().ToList();

        int pairCount = 0;
        foreach (var rank in uniqueRanks)
        {
            int count = ranks.Count(r => r == rank);
            if (count == 2) { hasPair = true; pairCount++; }
            if (count == 3) hasThreeOfAKind = true;
            if (count == 4) hasFourOfAKind = true;
            if (count == 5) hasFiveOfAKind = true;
        }
        if (pairCount >= 2) hasTwoPair = true;

        // ====== Flush ======
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            int suitCount = selectedCards.Count(c => c.Suit == suit);
            if (suitCount == selectedCards.Count && selectedCards.Count >= 5)
                isFlush = true;
        }

        // ====== Straight (mínimo 5 ranks distintos) ======
        if (uniqueRanks.Count >= 5)
        {
            var orderedRanks = uniqueRanks.OrderBy(r => (int)r).ToList();
            int consecutive = 1;

            for (int i = 1; i < orderedRanks.Count; i++)
            {
                if ((int)orderedRanks[i] == (int)orderedRanks[i - 1] + 1)
                {
                    consecutive++;
                    if (consecutive >= 5)
                    {
                        isStraight = true;
                        break;
                    }
                }
                else
                {
                    consecutive = 1;
                }
            }

            // Tratamento especial: Ás pode ser "1" ou "14" (A-2-3-4-5 ou 10-J-Q-K-A)
            if (uniqueRanks.Contains(Rank.Ace))
            {
                var lowAceRanks = orderedRanks.Select(r => r == Rank.Ace ? 14 : (int)r).OrderBy(r => r).ToList();
                consecutive = 1;
                for (int i = 1; i < lowAceRanks.Count; i++)
                {
                    if (lowAceRanks[i] == lowAceRanks[i - 1] + 1)
                    {
                        consecutive++;
                        if (consecutive >= 5)
                        {
                            isStraight = true;
                            break;
                        }
                    }
                    else
                    {
                        consecutive = 1;
                    }
                }
            }
        }

        // ====== Royal ======
        if (uniqueRanks.Contains(Rank.Ten) &&
            uniqueRanks.Contains(Rank.Jack) &&
            uniqueRanks.Contains(Rank.Queen) &&
            uniqueRanks.Contains(Rank.King) &&
            uniqueRanks.Contains(Rank.Ace))
        {
            isRoyal = true;
        }

        // ====== Definir mão ======
        HandType currentHand = HandType.HighCard;

        if (hasPair) currentHand = HandType.Pair;
        if (hasTwoPair) currentHand = HandType.TwoPair;
        if (hasThreeOfAKind) currentHand = HandType.ThreeOfAKind;
        if (isStraight) currentHand = HandType.Straight;
        if (isFlush) currentHand = HandType.Flush;
        if (hasPair && hasThreeOfAKind) currentHand = HandType.FullHouse;
        if (hasFourOfAKind) currentHand = HandType.FourOfAKind;
        if (isFlush && isStraight) currentHand = HandType.StraightFlush;
        if (isFlush && isRoyal) currentHand = HandType.RoyalFlush;
        if (hasFiveOfAKind) currentHand = HandType.FiveOfAKind;
        if (isFlush && hasThreeOfAKind && hasPair) currentHand = HandType.FlushHouse;
        if (isFlush && hasFiveOfAKind) currentHand = HandType.FlushFive;

        return currentHand;
    }
}
