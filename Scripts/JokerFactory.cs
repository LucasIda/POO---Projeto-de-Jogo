using Godot;
using System.Collections.Generic;

public static class JokerFactory
{
    public static List<JokerCard> CreateJokers()
    {
        var list = new List<JokerCard>();

      
        var clown = new JokerCard();
        clown.Initialize("Clown", GD.Load<Texture2D>("res://Sprites/Jokers/Clown.png"));
        clown.AddEffect(new EffectAddMultiplier(2));
        list.Add(clown);

       
        var coinMaster = new JokerCard();
        coinMaster.Initialize("Coin Master", GD.Load<Texture2D>("res://Sprites/Jokers/CoinMaster.png"));
        coinMaster.AddEffect(new EffectAddChips(50));
        list.Add(coinMaster);

        var doubler = new JokerCard();
        doubler.Initialize("Doubler", GD.Load<Texture2D>("res://Sprites/Jokers/Doubler.png"));
        doubler.AddEffect(new EffectMultiplyMultiplier(1.5f));
        list.Add(doubler);

        return list;
    }
}
