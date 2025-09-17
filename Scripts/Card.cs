using Godot;
using System;

public partial class Card : BaseCard
{
    public CardData Data { get; private set; }

    public void SetCard(CardData data, Texture2D texture)
    {
        Data = data;
        Initialize(data.Name, texture);
    }

    public int GetChipValue()
    {
        return Data.Rank switch
        {
            Rank.Ace => 11,
            Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => 10,
            _ => (int)Data.Rank
        };
    }
}
