using System;
using System.Collections.Generic;

public enum Suit
{
    Clubs,
    Hearts,
    Spades,
    Diamonds
}

public enum Rank
{
    Ace = 14,
    Two = 2,
    Three = 3,
    Four = 4,
    Five = 5,
    Six = 6,
    Seven = 7,
    Eight = 8,
    Nine = 9,
    Ten = 10,
    Jack = 11,
    Queen = 12,
    King = 13
}

public class CardData
{
    public Suit Suit { get; private set; }
    public Rank Rank { get; private set; }
    public string Name => $"{Rank} of {Suit}";
    public string TexturePath { get; private set; }

    public CardData(Suit suit, Rank rank, string texturePath)
    {
        Suit = suit;
        Rank = rank;
        TexturePath = texturePath;
    }
}

public static class CardDatabase
{
    public static List<CardData> GenerateDeck()
    {
        var deck = new List<CardData>();

        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                string texturePath = $"res://Sprites/Cartas/{GetFileName(rank, suit)}.png";
                deck.Add(new CardData(suit, rank, texturePath));
            }
        }

        return deck;
    }

    private static string GetFileName(Rank rank, Suit suit)
    {
        string rankStr = rank switch
        {
            Rank.Ace => "ace",
            Rank.Two => "two",
            Rank.Three => "three",
            Rank.Four => "four",
            Rank.Five => "five",
            Rank.Six => "six",
            Rank.Seven => "seven",
            Rank.Eight => "eight",
            Rank.Nine => "nine",
            Rank.Ten => "ten",
            Rank.Jack => "jack",
            Rank.Queen => "queen",
            Rank.King => "king",
            _ => ""
        };

        string suitStr = suit.ToString().ToLower(); // Clubs -> clubs

        return $"{rankStr}_of_{suitStr}";
    }
}
