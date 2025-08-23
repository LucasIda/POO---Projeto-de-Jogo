using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UIController : Control
{
    [Export] private PackedScene CardScene;
    [Export] private NodePath CardContainerPath;

    private Control _cardContainer;
    private List<CardData> _deck = new();
    private List<CardData> _discard = new();

    public override void _Ready()
    {
        _cardContainer = GetNode<Control>(CardContainerPath);

        // 🔹 Gerar baralho completo
        _deck = CardDatabase.GenerateDeck();

        // Embaralhar deck
        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        // Conectar botões
        GetNode<Button>("VBoxContainer/DrawButton").Pressed += OnDrawPressed;
        GetNode<Button>("VBoxContainer/ReturnButton").Pressed += OnReturnPressed;
        GetNode<Button>("VBoxContainer/ResetButton").Pressed += OnResetPressed;
    }

    private void OnDrawPressed()
    {
        if (_deck.Count == 0)
        {
            GD.Print("O baralho acabou!");
            return;
        }

        CardData cardData = _deck[0];
        _deck.RemoveAt(0);
        _discard.Add(cardData);

        var card = CardScene.Instantiate<Card>();
        Texture2D texture = GD.Load<Texture2D>(cardData.TexturePath);
        card.SetCard(cardData, texture);

        card.OnCardClicked += OnCardClicked;

        float cardOffsetX = _cardContainer.GetChildCount() * 110;
        card.Position = new Vector2(cardOffsetX, 0);

        _cardContainer.AddChild(card);

        GD.Print($"Carta sacada: {cardData.Name}");
    }

    private void OnCardClicked(Card clickedCard)
    {
        GD.Print($"Carta {clickedCard.Data.Name} foi clicada!");
        clickedCard.ToggleSelection();
    }

    private void OnReturnPressed()
    {
        if (_discard.Count == 0)
        {
            GD.Print("Não há cartas para devolver.");
            return;
        }

        CardData lastCard = _discard.Last();
        _discard.RemoveAt(_discard.Count - 1);
        _deck.Insert(0, lastCard);

        GD.Print($"Devolveu: {lastCard.Name}");

        if (_cardContainer.GetChildCount() > 0)
        {
            Node lastChild = _cardContainer.GetChild(_cardContainer.GetChildCount() - 1);
            lastChild.QueueFree();

            for (int i = 0; i < _cardContainer.GetChildCount(); i++)
            {
                if (_cardContainer.GetChild(i) is Card card)
                    card.Position = new Vector2(i * 110, 0);
            }
        }
    }

    private void OnResetPressed()
    {
        _deck.AddRange(_discard);
        _discard.Clear();

        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        GD.Print("Baralho resetado!");
        ClearCardContainer();
    }

    private void ClearCardContainer()
    {
        foreach (Node child in _cardContainer.GetChildren())
            child.QueueFree();
    }
}
