using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UIController : Control
{
    [Export] private PackedScene CardScene;
    [Export] private NodePath CardContainerPath;
    [Export] private NodePath HandNameLabelPath;

    private Control _cardContainer;
    private Label _handNameLabel;

    private List<CardData> _deck = new();
    private List<CardData> _discard = new();

    // Lista de cartas atualmente selecionadas
    private List<Card> _selectedCards = new();

    // Limite máximo de cartas na mão
    //private const int MAX_HAND_SIZE = 8;

    public override void _Ready()
    {
        _cardContainer = GetNode<Control>(CardContainerPath);

        // Tenta pegar o Label via NodePath exportado; se não estiver setado, tenta caminho padrão.
        _handNameLabel = GetNodeOrNull<Label>(HandNameLabelPath);
        if (_handNameLabel == null)
        _handNameLabel = GetNodeOrNull<Label>("Panel/HandData/HandName");


        UpdateCurrentHandLabel(); // estado inicial

        // Gerar baralho completo
        _deck = CardDatabase.GenerateDeck();

        // Embaralhar deck
        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        // Conectar botões
        GetNode<Button>("VBoxContainer/DrawButton").Pressed += OnDrawPressed;
        GetNode<Button>("VBoxContainer/ReturnButton").Pressed += OnReturnPressed;
        GetNode<Button>("VBoxContainer/ResetButton").Pressed += OnResetPressed;

        // Comprar automaticamente 8 cartas no início
        /*for (int i = 0; i < MAX_HAND_SIZE; i++)
        {
            DrawCard();
        }
        */    
    }

    private void OnDrawPressed()
    {
        DrawCard();
    }

    private void DrawCard()
    {
        /* Impede de passar de 8 cartas
        if (_cardContainer.GetChildCount() >= MAX_HAND_SIZE)
        {
            GD.Print("Mão cheia! Não é possível comprar mais cartas.");
            return;
        }
        */
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

        if (clickedCard.IsSelected)
        {
            if (!_selectedCards.Contains(clickedCard))
                _selectedCards.Add(clickedCard);
        }
        else
        {
            _selectedCards.Remove(clickedCard);
        }

        // Atualiza label da mão atual
        UpdateCurrentHandLabel();
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

        // Limpa lista de selecionadas que não existem mais
        _selectedCards.RemoveAll(c => !IsInstanceValid(c));

        // Atualiza label da mão atual
        UpdateCurrentHandLabel();
    }
    private void OnResetPressed()
    {
        _deck.AddRange(_discard);
        _discard.Clear();

        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        GD.Print("Baralho resetado!");
        ClearCardContainer();
        _selectedCards.Clear();

        /* Recompra automática das 8 cartas após reset
        for (int i = 0; i < MAX_HAND_SIZE; i++)
        {
            DrawCard();
        }
        */
        // Nenhuma carta selecionada após reset
        UpdateCurrentHandLabel();
    }
    private void ClearCardContainer()
    {
        foreach (Node child in _cardContainer.GetChildren())
            child.QueueFree();
    }
    // === Atualiza o Label com a mão atual (baseada nas cartas selecionadas) ===
    private void UpdateCurrentHandLabel()
    {
        if (_handNameLabel == null) return;

        if (_selectedCards.Count > 0)
        {
            var selectedData = _selectedCards.Select(c => c.Data).ToList();
            var hand = HandEvaluator.EvaluateHand(selectedData);
            _handNameLabel.Text = hand.ToString();
            GD.Print($"Mão atual: {hand}");
        }
        else
        {
            _handNameLabel.Text = "Nenhuma mão";
            GD.Print("Nenhuma carta selecionada.");
        }
    }
}
