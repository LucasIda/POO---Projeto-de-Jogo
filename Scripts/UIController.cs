using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UIController : Control
{
    [Export] private PackedScene CardScene;
    [Export] private NodePath CardContainerPath;
    [Export] private NodePath HandNameLabelPath;
    [Export] private Button DrawButton;
    [Export] private Button DiscardButton;
    [Export] private Button PlayButton;
    [Export] private Button ResetButton;
    [Export] private Label RoundScoreLabel;
    [Export] private NodePath DeckViewPath;

    private Control _cardContainer;
    private DeckView _deckView;
    private Label _handNameLabel;
    private Label _chipsLabel;
    private Label _multLabel;

    private List<CardData> _deck = new();
    private List<CardData> _discard = new();
    private List<Card> _hand = new();
    private List<Card> _selectedCards = new();
    private List<Card> _discardPile = new();

    private int _roundScore = 0;
    private int _totalDeckCount;

    public override void _Ready()
    {
        _cardContainer = GetNode<Control>(CardContainerPath);
        _chipsLabel = GetNode<Label>("Panel/ScoreBox/Chip/ChipLabel");
        _multLabel = GetNode<Label>("Panel/ScoreBox/Mult/MultLabel");
        RoundScoreLabel = GetNode<Label>("Panel/RoundScore/ScorePanel/HBoxContainer/ScoreLabel");
        _handNameLabel = GetNodeOrNull<Label>(HandNameLabelPath) ?? GetNode<Label>("Panel/HandData/HandName");
        _deckView = GetNode<DeckView>("DeckView");

        InitDeck();
        _deckView.UpdateCount(_deck.Count, _totalDeckCount); // Atualiza visual do deck ao iniciar
        UpdateCurrentHandLabel();

        if (DrawButton != null) DrawButton.Pressed += () => DrawCards(1);
        if (DiscardButton != null) DiscardButton.Pressed += OnDiscardPressed;
        if (PlayButton != null) PlayButton.Pressed += OnPlayPressed;
        if (ResetButton != null) ResetButton.Pressed += OnResetPressed;
    }

    private void InitDeck()
    {
        string basePath = "res://Sprites/Cartas/";
        foreach (Suit suit in Enum.GetValues(typeof(Suit)))
        {
            foreach (Rank rank in Enum.GetValues(typeof(Rank)))
            {
                string texturePath = $"{basePath}{rank.ToString().ToLower()}_of_{suit.ToString().ToLower()}.png";
                _deck.Add(new CardData(suit, rank, texturePath));
            }
        }
        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        _totalDeckCount = _deck.Count;
    }

    private void DrawCard()
    {
        if (_deck.Count == 0)         
            return;

        CardData cardData = _deck[0];
        _deck.RemoveAt(0);

        var card = CardScene.Instantiate<Card>();
        card.SetCard(cardData, GD.Load<Texture2D>(cardData.TexturePath));
        card.OnCardClicked += OnCardClicked;
        card.OnDragging += OnCardDragging;
        card.OnDragEnded += OnCardDragEnded;

        _hand.Add(card);
        _cardContainer.AddChild(card);

        _deckView.UpdateCount(_deck.Count, _totalDeckCount); // Atualiza sprites do deck
        UpdateHandVisuals();
        GD.Print($"Carta sacada: {cardData.Name}");
    }

    private void DrawCards(int count)
    {
        if (_deck.Count == 0)
        {
            GD.Print("O baralho acabou!");
            return;
        }

        for (int i = 0; i < count && _deck.Count > 0; i++)
            DrawCard();
    }

    private void OnCardClicked(Card clickedCard)
    {
        clickedCard.ToggleSelection();

        if (clickedCard.IsSelected && !_selectedCards.Contains(clickedCard))
            _selectedCards.Add(clickedCard);
        else if (!clickedCard.IsSelected)
            _selectedCards.Remove(clickedCard);

        UpdateCurrentHandLabel();
    }

    private void OnPlayPressed()
    {
        if (_selectedCards.Count == 0) return;
        GD.Print("Cartas jogadas:");

        var selectedData = _selectedCards.Select(c => c.Data).Where(d => d != null).ToList();
        if (selectedData.Count == 0) return;

        var handEval = HandChecker.EvaluateHand(selectedData);
        int chips = HandValue.GetChips(handEval);
        int mult = HandValue.GetMultiplier(handEval);
        int score = HandValue.GetScore(handEval);

        _roundScore += score;
        RoundScoreLabel.Text = $"{_roundScore}";

        foreach (var card in _selectedCards)
        {
            _hand.Remove(card);
            _discardPile.Add(card);
            GD.Print($" - {card.Data.Rank} of {card.Data.Suit}");
            card.QueueFree();
        }

        DrawCards(selectedData.Count);
        _selectedCards.Clear();
        UpdateHandVisuals();
        UpdateCurrentHandLabel();
    }

    private void OnDiscardPressed()
    {
        if (_selectedCards.Count == 0) return;
        GD.Print("Cartas descartadas:");

        int toDraw = Math.Min(_selectedCards.Count, _deck.Count);

        foreach (var card in _selectedCards)
        {
            _hand.Remove(card);
            _discard.Add(card.Data);
            _discardPile.Add(card);
            GD.Print($" - {card.Data.Rank} of {card.Data.Suit}");
            card.QueueFree();
        }

        _selectedCards.Clear();

        if (toDraw > 0)
            DrawCards(toDraw);

        UpdateHandVisuals();
        UpdateCurrentHandLabel();
    }

    private void OnResetPressed()
    {
        GD.Print("Resetando deck...");

        _deck.Clear();
        InitDeck();

        _hand.Clear();
        _selectedCards.Clear();
        _discardPile.Clear();

        ClearCardContainer();

        _roundScore = 0;
        RoundScoreLabel.Text = "0";

        _deckView.UpdateCount(_deck.Count, _totalDeckCount);
        UpdateCurrentHandLabel();

        GD.Print($"Deck resetado. Total de cartas no deck: {_deck.Count}");
    }


    private void ClearCardContainer()
    {
        foreach (Node child in _cardContainer.GetChildren())
            child.QueueFree();
    }

    private void UpdateHandVisuals()
    {
        float spacing = 57;
        for (int i = 0; i < _hand.Count; i++)
        {
            if (!_hand[i].IsDragging)
                _hand[i].Position = new Vector2(i * spacing, 0);
        }
    }

    private void UpdateCurrentHandLabel()
    {
        if (_handNameLabel == null) return;

        if (_selectedCards.Count > 0)
        {
            var selectedData = _selectedCards.Select(c => c.Data).ToList();
            var handEval = HandChecker.EvaluateHand(selectedData);
            int chips = HandValue.GetChips(handEval);
            int mult = HandValue.GetMultiplier(handEval);
            int score = HandValue.GetScore(handEval);

            _handNameLabel.Text = $"{handEval}";
            _chipsLabel.Text = $"{chips}";
            _multLabel.Text = $"{mult}";
            GD.Print($"Mão atual: {handEval}, Chips: {chips}, Mult: {mult}, Score: {score}");
        }
        else
        {
            _handNameLabel.Text = "";
            _chipsLabel.Text = "0";
            _multLabel.Text = "0";
        }
    }

    private void OnCardDragging(Card card, Vector2 delta)
    {
        // Implementar lógica adicional se necessário
    }

    private void OnCardDragEnded(Card card)
    {
        _hand = _hand.OrderBy(c => c.Position.X).ToList();
        UpdateHandVisuals();
    }
}
