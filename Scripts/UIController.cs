using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UIController : Control
{
    [Export] private PackedScene CardScene;
    [Export] private NodePath CardContainerPath;
    [Export] private NodePath HandNameLabelPath;
    [Export] private Button _playButton;
    [Export] private Label _roundScoreLabel;

    private Control _cardContainer;
    private Label _handNameLabel;
    private Label _chipsLabel;
    private Label _multLabel;

    private List<CardData> _deck = new();
    private List<CardData> _discard = new();
    private List<Card> _selectedCards = new();
    private List<Card> _discardPile = new();
    
    private int _roundScore = 0;

    public override void _Ready()
    {
        _cardContainer = GetNode<Control>(CardContainerPath);
        _chipsLabel = GetNode<Label>("Panel/ScoreBox/Chip/ChipLabel");
        _multLabel = GetNode<Label>("Panel/ScoreBox/Mult/MultLabel");
        _roundScoreLabel = GetNode<Label>("Panel/RoundScore/ScorePanel/HBoxContainer/ScoreLabel");
        _handNameLabel = GetNodeOrNull<Label>(HandNameLabelPath);

        if (_handNameLabel == null)
            _handNameLabel = GetNodeOrNull<Label>("Panel/HandData/HandName");

        UpdateCurrentHandLabel(); // estado inicial

        _deck = CardDatabase.GenerateDeck();
        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        GetNode<Button>("VBoxContainer/DrawButton").Pressed += OnDrawPressed;
        GetNode<Button>("VBoxContainer/DiscardButton").Pressed += OnDiscardPressed;
        GetNode<Button>("VBoxContainer/ResetButton").Pressed += OnResetPressed;
        GetNode<Button>("VBoxContainer/PlayButton").Pressed += OnPlayPressed;
    }

    private void OnPlayPressed()
    {
        if (_selectedCards.Count == 0)
            return;

        // Verifica se todos os CardData são válidos
        var selectedData = _selectedCards.Select(c => c.Data).Where(d => d != null).ToList();
        if (selectedData.Count == 0)
            return; // Nenhum CardData válido, sai da função

        // Avalia a mão
        var hand = HandChecker.EvaluateHand(selectedData);

        int chips = HandValue.GetChips(hand);
        int mult = HandValue.GetMultiplier(hand);
        int score = HandValue.GetScore(hand);

        _roundScore += score;
        if (_roundScoreLabel != null)
            _roundScoreLabel.Text = $"{_roundScore}";

        // Move cartas para o descarte
        foreach (var card in _selectedCards)
        {
            if (card != null)
            {
                _discardPile.Add(card);
                card.QueueFree();
            }
        }
        _selectedCards.Clear();

        // Comprar cartas novas
        DrawCards(selectedData.Count);
        UpdateCurrentHandLabel();
    }


    private void OnDrawPressed()
    {
        DrawCard();
    }

    private void DrawCard()
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

    private void DrawCards(int count)
    {
        for (int i = 0; i < count; i++)
        {
            if (_deck.Count > 0)
                DrawCard();
        }
    }

    private void OnCardClicked(Card clickedCard)
    {
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

        UpdateCurrentHandLabel();
    }

    private void OnSwitchPressed()
    {
        
    }

    private void OnDiscardPressed()
    {
        if (_discard.Count == 0)
        {
            return;
        }

        int discardCount = _selectedCards.Count;

        GD.Print("Cartas descartadas:");

        foreach (var card in _selectedCards)
        {
            if (card != null)
            {
                _discardPile.Add(card);
                GD.Print($" - {card.Data.Rank} of {card.Data.Suit}");
                card.QueueFree();
            }
        }
        _selectedCards.Clear();

        int availableToDraw = Math.Min(discardCount, _deck.Count);

        if (availableToDraw > 0)
            DrawCards(availableToDraw);

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
            var hand = HandChecker.EvaluateHand(selectedData);

            int chips = HandValue.GetChips(hand);
            int mult = HandValue.GetMultiplier(hand);
            int score = HandValue.GetScore(hand);

            _handNameLabel.Text = $"{hand}";
            GD.Print($"Mão atual: {hand}");
            GD.Print($"Hand avaliada: {hand}, Chips: {chips}, Mult: {mult} = {score}");

            _chipsLabel.Text = $"{chips}";
            _multLabel.Text = $"{mult}";
        }
        else
        {
            _handNameLabel.Text = "";
            _chipsLabel.Text = "0";
            _multLabel.Text = "0";
            GD.Print("Nenhuma carta selecionada.");
        }
    }
}
