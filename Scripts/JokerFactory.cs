using Godot;
using System;
using System.Collections.Generic;

public static class JokerFactory
{
    // Agora ele precisa da cena base do Curinga para instanciar
    public static List<JokerCard> CreateJokers(PackedScene jokerScene, Func<int> getPlayerCoins)
    {
        if (jokerScene == null)
        {
            GD.PrintErr("JokerFactory: A PackedScene do Curinga está nula!");
            return new List<JokerCard>();
        }
        
        var list = new List<JokerCard>();

        // Clown
        // Em vez de 'new JokerCard()', usamos 'jokerScene.Instantiate<JokerCard>()'
        var clown = jokerScene.Instantiate<JokerCard>();
        clown.Initialize("Clown", GD.Load<Texture2D>("res://Sprites/Jokers/Clown.png"));
        clown.Rarity = JokerRarity.Common;
        clown.AddEffect(new EffectAddMultiplier(2, "Double your base multiplier"));
        list.Add(clown);

       
        // Coin Master
        var coinMaster = jokerScene.Instantiate<JokerCard>();
        coinMaster.Initialize("Coin Master", GD.Load<Texture2D>("res://Sprites/Jokers/CoinMaster.png"));
        coinMaster.Rarity = JokerRarity.Uncommon;
        coinMaster.AddEffect(new EffectAddChips(50, "Add 50 chips to your base chips"));
        list.Add(coinMaster);

        // Doubler
        var doubler = jokerScene.Instantiate<JokerCard>();
        doubler.Initialize("Doubler", GD.Load<Texture2D>("res://Sprites/Jokers/Doubler.png"));
        doubler.Rarity = JokerRarity.Rare;
        doubler.AddEffect(new EffectMultiplyMultiplier(1.5f, "Increase your base multiplier by 50%"));
        list.Add(doubler);

        // Bull
        var bull = jokerScene.Instantiate<JokerCard>();
        bull.Initialize("Bull", GD.Load<Texture2D>("res://Sprites/Jokers/Bull.png"));
        bull.Rarity = JokerRarity.Common;
        bull.AddEffect(new EffectAddChipsPerCoin(2,getPlayerCoins,"Gain 2 Chips for each $1 you have"));
        list.Add(bull);

        return list;
    }
}