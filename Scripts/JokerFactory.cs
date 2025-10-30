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

        // Hallucination
        var hallucination = jokerScene.Instantiate<JokerCard>();
        hallucination.Initialize("Hallucinate", GD.Load<Texture2D>("res://Sprites/Jokers/Hallucination.png"));
        hallucination.Rarity = JokerRarity.Uncommon;
        hallucination.AddEffect(new EffectAddMultiplier(3, "Triples your base multiplier"));
        list.Add(hallucination);

        // Hit the Road
        var hittheroad = jokerScene.Instantiate<JokerCard>();
        hittheroad.Initialize("Hit the Road", GD.Load<Texture2D>("res://Sprites/Jokers/HittheRoad.png"));
        hittheroad.Rarity = JokerRarity.Rare;
        hittheroad.AddEffect(new EffectAddMultiplier(5, "5x your base multiplier"));
        list.Add(hittheroad);

        // Egg
        var egg = jokerScene.Instantiate<JokerCard>();
        egg.Initialize("Egg", GD.Load<Texture2D>("res://Sprites/Jokers/Egg.png"));
        egg.Rarity = JokerRarity.Common;
        egg.AddEffect(new EffectAddChips(25, "Add 25 chips to your base chips"));
        list.Add(egg);

        // Coin Master
        var coinMaster = jokerScene.Instantiate<JokerCard>();
        coinMaster.Initialize("Coin Master", GD.Load<Texture2D>("res://Sprites/Jokers/CoinMaster.png"));
        coinMaster.Rarity = JokerRarity.Uncommon;
        coinMaster.AddEffect(new EffectAddChips(50, "Add 50 chips to your base chips"));
        list.Add(coinMaster);

        // Blue Joker
        var bluejoker = jokerScene.Instantiate<JokerCard>();
        bluejoker.Initialize("Blue Joker", GD.Load<Texture2D>("res://Sprites/Jokers/BlueJoker.png"));
        bluejoker.Rarity = JokerRarity.Rare;
        bluejoker.AddEffect(new EffectAddChips(90, "Add 90 chips to your base chips"));
        list.Add(bluejoker);

        // Bull
        var bull = jokerScene.Instantiate<JokerCard>();
        bull.Initialize("Bull", GD.Load<Texture2D>("res://Sprites/Jokers/Bull.png"));
        bull.Rarity = JokerRarity.Common;
        bull.AddEffect(new EffectAddChipsPerCoin(2,getPlayerCoins,"Gain 2 Chips for each $1 you have"));
        list.Add(bull);

        // Stuntman
        var stuntman = jokerScene.Instantiate<JokerCard>();
        stuntman.Initialize("Stuntman", GD.Load<Texture2D>("res://Sprites/Jokers/Stuntman.png"));
        stuntman.Rarity = JokerRarity.Rare;
        stuntman.AddEffect(new EffectAddChipsPerCoin(5, getPlayerCoins, "Gain 5 chips for each $1 you have"));
        list.Add(stuntman);

        // Doubler
        var doubler = jokerScene.Instantiate<JokerCard>();
        doubler.Initialize("Doubler", GD.Load<Texture2D>("res://Sprites/Jokers/Doubler.png"));
        doubler.Rarity = JokerRarity.Uncommon;
        doubler.AddEffect(new EffectMultiplyMultiplier(1.5f, "Increase your base multiplier by 50%"));
        list.Add(doubler);

        // The Family
        var thefamily = jokerScene.Instantiate<JokerCard>();
        thefamily.Initialize("The Family", GD.Load<Texture2D>("res://Sprites/Jokers/TheFamily.png"));
        thefamily.Rarity = JokerRarity.Rare;
        thefamily.AddEffect(new EffectMultiplyMultiplier(2.5f, "Increase your base multplier by 150%"));
        list.Add(thefamily);
        

        return list;
    }
}