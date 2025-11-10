using Godot;
using System;
using System.Collections.Generic;

public static class JokerFactory
{
    // Agora ele precisa da cena base do Curinga para instanciar
    public static List<JokerCard> CreateJokers(PackedScene jokerScene, Func<int> getPlayerCoins, Action<int> addPlayerMoney)
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

        // Blainstorm
        var brainstorm = jokerScene.Instantiate<JokerCard>();
        brainstorm.Initialize("Blainstorm", GD.Load<Texture2D>("res://Sprites/Jokers/Brainstorm.png"));
        brainstorm.Rarity = JokerRarity.Rare;
        brainstorm.AddEffect(new EffectAddChips(150, "Add 150 chips to your base chips"));
        list.Add(brainstorm);

        // Bull
        var bull = jokerScene.Instantiate<JokerCard>();
        bull.Initialize("Bull", GD.Load<Texture2D>("res://Sprites/Jokers/Bull.png"));
        bull.Rarity = JokerRarity.Common;
        bull.AddEffect(new EffectAddChipsPerCoin(2,getPlayerCoins,"Gain 2 Chips for each $1 you have"));
        list.Add(bull);

        // Polychrome
        var Polychrome = jokerScene.Instantiate<JokerCard>();
        Polychrome.Initialize("Polychrome", GD.Load<Texture2D>("res://Sprites/Jokers/Polychrome.png"));
        Polychrome.Rarity = JokerRarity.Rare;
        Polychrome.AddEffect(new EffectAddChipsPerCoin(7,getPlayerCoins,"Gain 7 Chips for each $1 you have"));
        list.Add(Polychrome);

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

        //Lucky Cat
        var luckycat = jokerScene.Instantiate<JokerCard>();
        luckycat.Initialize("Lucky Cat", GD.Load<Texture2D>("res://Sprites/Jokers/Lucky_Cat.png"));
        luckycat.Rarity = JokerRarity.Rare;
        luckycat.AddEffect(new EffectMultiplyMultiplier(2f, "Increase your base multplier by 200%"));
        list.Add(luckycat);
        

        // The Family
        var thefamily = jokerScene.Instantiate<JokerCard>();
        thefamily.Initialize("The Family", GD.Load<Texture2D>("res://Sprites/Jokers/TheFamily.png"));
        thefamily.Rarity = JokerRarity.Rare;
        thefamily.AddEffect(new EffectMultiplyMultiplier(2.5f, "Increase your base multplier by 150%"));
        list.Add(thefamily);

        // Kenzo
        var Kenzo = jokerScene.Instantiate<JokerCard>();
        Kenzo.Initialize("Kenzo", GD.Load<Texture2D>("res://Sprites/Jokers/Kenzo.png"));
        Kenzo.Rarity = JokerRarity.Legendary;
        Kenzo.AddEffect(new EffectMultiplyMultiplier(10.0f, "Increase your base multplier by 1000%"));
        list.Add(Kenzo);

        // Pedro
        var Pedro = jokerScene.Instantiate<JokerCard>();
        Pedro.Initialize("Pedro", GD.Load<Texture2D>("res://Sprites/Jokers/Pedro.png"));
        Pedro.Rarity = JokerRarity.Legendary;
        Pedro.AddEffect(new EffectAddMultiplier(0, "???"));
        list.Add(Pedro);

        // Diogo
        var Diogo = jokerScene.Instantiate<JokerCard>();
        Diogo.Initialize("Diogo", GD.Load<Texture2D>("res://Sprites/Jokers/Diogo.png"));
        Diogo.Rarity = JokerRarity.Legendary;
        Diogo.AddEffect(new EffectAddChips(500, "Add 500 chips to your base chips"));
        list.Add(Diogo);

        // Murilo
        var Murilo = jokerScene.Instantiate<JokerCard>();
        Murilo.Initialize("Murilo", GD.Load<Texture2D>("res://Sprites/Jokers/Murilo.png"));
        Murilo.Rarity = JokerRarity.Legendary;
        Murilo.AddEffect(new EffectAddChipsPerCoin(30,getPlayerCoins,"Gain 30 Chips for each $1 you have"));
        list.Add(Murilo);

        // Fabricio
        var Fabricio = jokerScene.Instantiate<JokerCard>();
        Fabricio.Initialize("Fabricio", GD.Load<Texture2D>("res://Sprites/Jokers/Fabricio.png"));
        Fabricio.Rarity = JokerRarity.Legendary;
        Fabricio.AddEffect(new EffectAddMultiplier(40, "Add 40 to your base multiplier"));
        list.Add(Fabricio);
       
       
        var greedyJoker = jokerScene.Instantiate<JokerCard>();
        greedyJoker.Initialize("Greedy Joker", GD.Load<Texture2D>("res://Sprites/Jokers/GreedyJoker.png"));
        greedyJoker.Rarity = JokerRarity.Uncommon;
        var diamondFilter = new FilterBySuit(Suit.Diamonds);
        greedyJoker.AddEffect(new EffectAddMultPerFilteredCard(3,diamondFilter,"+3 Mult for each Diamond card played" ));
        list.Add(greedyJoker);
        
        var lustyJoker = jokerScene.Instantiate<JokerCard>();
        lustyJoker.Initialize("Lusty Joker", GD.Load<Texture2D>("res://Sprites/Jokers/LustyJoker.png"));
        lustyJoker.Rarity = JokerRarity.Uncommon;
        var heartsFilter = new FilterBySuit(Suit.Hearts);
        lustyJoker.AddEffect(new EffectAddMultPerFilteredCard(3,heartsFilter,"+3 Mult for each Heart card played"));
        list.Add(lustyJoker);

        var wrathfulJoker = jokerScene.Instantiate<JokerCard>();
        wrathfulJoker.Initialize("Wrathful Joker", GD.Load<Texture2D>("res://Sprites/Jokers/WrathfulJoker.png")); 
        wrathfulJoker.Rarity = JokerRarity.Uncommon;
        var spadeFilter = new FilterBySuit(Suit.Spades);
        wrathfulJoker.AddEffect(new EffectAddMultPerFilteredCard(3,spadeFilter,"+3 Mult for each Spade card played"));
        list.Add(wrathfulJoker);

        var gluttonousJoker = jokerScene.Instantiate<JokerCard>();
        gluttonousJoker.Initialize("Gluttonous Joker", GD.Load<Texture2D>("res://Sprites/Jokers/GluttonousJoker.png"));
        gluttonousJoker.Rarity = JokerRarity.Uncommon;
        var clubFilter = new FilterBySuit(Suit.Clubs);
        gluttonousJoker.AddEffect(new EffectAddMultPerFilteredCard(3,clubFilter,"+3 Mult for each Club card played"));
        list.Add(gluttonousJoker);

        var Scholar = jokerScene.Instantiate<JokerCard>();
        Scholar.Initialize("Tri-Ace", GD.Load<Texture2D>("res://Sprites/Jokers/Scholar.png"));
        Scholar.Rarity = JokerRarity.Rare;
        var aceCounterFilter = new FilterByRank(Rank.Ace);
        Scholar.AddEffect(new EffectAddMultPerFilteredCard(4,aceCounterFilter,"+4 Mult for each Ace card played"));
        list.Add(Scholar);
        
        var bloodstone = jokerScene.Instantiate<JokerCard>();
        bloodstone.Initialize("Bloodstone", GD.Load<Texture2D>("res://Sprites/Jokers/Bloodstone.png")); 
        bloodstone.Rarity = JokerRarity.Rare;
        var heartsCounterFilter = new FilterBySuit(Suit.Hearts);
        bloodstone.AddEffect(new EffectMultiplyMultiplierPerFilteredCard(2, heartsCounterFilter,"x2 Mult for each Heart card played"));
        list.Add(bloodstone);

        var cavendish = jokerScene.Instantiate<JokerCard>();
        cavendish.Initialize("Cavendish", GD.Load<Texture2D>("res://Sprites/Jokers/Cavendish.png"));
        cavendish.Rarity = JokerRarity.Rare;
        cavendish.AddEffect(new EffectMultiplyMultiplier(3,"Triple your base multiplier"));
        list.Add(cavendish);

        var RoughGem = jokerScene.Instantiate<JokerCard>();
        RoughGem.Initialize("Rough Gem", GD.Load<Texture2D>("res://Sprites/Jokers/RoughGem.png"));
        RoughGem.Rarity = JokerRarity.Common;
        RoughGem.AddEffect(new EffectAddMoneyPerFilteredCard(1,diamondFilter,addPlayerMoney,"+$1 for each Diamond card played" ));
        list.Add(RoughGem);

        return list;
    }
}