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
    [Export] private Button SuitSortButton;
    [Export] private Button RankSortButton;
    [Export] private Label RoundScoreLabel;
    [Export] private NodePath DeckViewPath;
    [Export] private Label DiscardLeftLabel;
    [Export] private Label PlayLeftLabel;

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
    private const int MaxHandSize = 8;
    private const int MaxSelectedCards = 5;
    private const int MaxDiscards = 3;
    private const int MaxPlays = 4;
    private int _discardCount = 0;
    private int _playCount = 0;

    public override void _Ready()
    {
        _cardContainer = GetNode<Control>(CardContainerPath);
        _chipsLabel = GetNode<Label>("Panel/ScoreBox/Chip/ChipLabel");
        _multLabel = GetNode<Label>("Panel/ScoreBox/Mult/MultLabel");
        RoundScoreLabel = GetNode<Label>("Panel/RoundScore/ScorePanel/HBoxContainer/ScoreLabel");
        _handNameLabel = GetNodeOrNull<Label>(HandNameLabelPath) ?? GetNode<Label>("Panel/HandData/HandName");
        _deckView = GetNode<DeckView>("DeckView");
        DiscardLeftLabel = GetNode<Label>("Panel/PlayDiscardCount/Discard/DiscardLeftLabel");
        PlayLeftLabel = GetNode<Label>("Panel/PlayDiscardCount/Play/PlayLeftLabel");

        InitDeck();
        _deckView.UpdateCount(_deck.Count, _totalDeckCount); // Atualiza visual do deck ao iniciar
        UpdateCurrentHandLabel();

        if (DrawButton != null) DrawButton.Pressed += () => DrawCards(1);
        UpdateDrawButtonState();
        if (DiscardButton != null) DiscardButton.Pressed += OnDiscardPressed;
        if (PlayButton != null) PlayButton.Pressed += OnPlayPressed;
        if (ResetButton != null) ResetButton.Pressed += OnResetPressed;
        if (SuitSortButton != null) SuitSortButton.Pressed += SortBySuit;
        if (RankSortButton != null) RankSortButton.Pressed += SortByRank;

        DrawCards(MaxHandSize);
        UpdateHandVisuals();
        UpdateActionCountersUI();
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
        if (_hand.Count >= MaxHandSize)
        {
            GD.Print("Mão cheia! Não é possível sacar mais cartas.");
            UpdateDrawButtonState();
            return;
        }

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

        _deckView.UpdateCount(_deck.Count, _totalDeckCount);
        UpdateHandVisuals();
        UpdateDrawButtonState();
        GD.Print($"Carta sacada: {cardData.Name}");
    }

    private void DrawCards(int count)
    {
        if (_deck.Count == 0)
        {
            GD.Print("O baralho acabou!");
            return;
        }

        int spaceLeft = MaxHandSize - _hand.Count;
        if (spaceLeft <= 0)
        {
            GD.Print("Mão cheia! Não é possível sacar mais cartas.");
            UpdateDrawButtonState();
            return;
        }

        int toDraw = Math.Min(count, Math.Min(spaceLeft, _deck.Count));
        for (int i = 0; i < toDraw; i++)
            DrawCard();

        UpdateDrawButtonState();
    }

    private void OnCardClicked(Card clickedCard)
    {
        if (clickedCard.IsSelected)
        {
            clickedCard.ToggleSelection();
            _selectedCards.Remove(clickedCard);
        }
        else
        {
            if (_selectedCards.Count >= MaxSelectedCards)
            {
                GD.Print($"Você só pode selecionar até {MaxSelectedCards} cartas.");
                return;
            }

            clickedCard.ToggleSelection();
            _selectedCards.Add(clickedCard);
        }

        UpdateCurrentHandLabel();
    }

    private void OnPlayPressed()
{
    if (_playCount >= MaxPlays)
    {
        GD.Print($"Você já jogou o máximo de {MaxPlays} vezes nesta rodada.");
        return;
    }

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

    // 👉 aqui é onde integramos com o GameManager
    GetNode<GameManager>("GameManager").AddChips(score);

    foreach (var card in _selectedCards)
    {
        _hand.Remove(card);
        _discardPile.Add(card);
        GD.Print($" - {card.Data.Rank} of {card.Data.Suit}");
        card.QueueFree();
    }

    DrawCards(selectedData.Count);

    _playCount++;
    _selectedCards.Clear();
    UpdateHandVisuals();
    UpdateCurrentHandLabel();
    UpdateDrawButtonState();
    UpdateActionCountersUI();
}


    private void OnDiscardPressed()
    {
        if (_discardCount >= MaxDiscards)
        {
            GD.Print($"Você já descartou o máximo de {MaxDiscards} vezes nesta rodada.");
            return;
        }

        if (_selectedCards.Count == 0) return;
        GD.Print("Cartas descartadas:");

        int requested = _selectedCards.Count;

        foreach (var card in _selectedCards)
        {
            _hand.Remove(card);
            _discard.Add(card.Data);
            _discardPile.Add(card);
            GD.Print($" - {card.Data.Rank} of {card.Data.Suit}");
            card.QueueFree();
        }

        _selectedCards.Clear();

        if (_deck.Count > 0)
            DrawCards(requested);

        _discardCount++;
        UpdateHandVisuals();
        UpdateCurrentHandLabel();
        UpdateDrawButtonState();
        UpdateActionCountersUI();
    }

    private void OnResetPressed()
    {
        GD.Print("Resetando deck...");

        GetNode<GameManager>("GameManager").ResetGlobalChips();

        _deck.Clear();
        InitDeck();

        _hand.Clear();
        _selectedCards.Clear();
        _discardPile.Clear();

        ClearCardContainer();

        _roundScore = 0;
        RoundScoreLabel.Text = "0";

        _discardCount = 0;
        _playCount = 0;

        _deckView.UpdateCount(_deck.Count, _totalDeckCount);
        UpdateCurrentHandLabel();

        GD.Print($"Deck resetado. Total de cartas no deck: {_deck.Count}");

        UpdateDrawButtonState();
        UpdateActionCountersUI();
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

    private void OnCardDragging(Card card, Vector2 delta) { }

    private void OnCardDragEnded(Card card)
    {
        _hand = _hand.OrderBy(c => c.Position.X).ToList();
        UpdateHandVisuals();
    }

    private void UpdateActionCountersUI()
    {
        int discardsLeft = Math.Max(0, MaxDiscards - _discardCount);
        int playsLeft = Math.Max(0, MaxPlays - _playCount);

        if (DiscardLeftLabel != null) DiscardLeftLabel.Text = discardsLeft.ToString();
        if (PlayLeftLabel != null) PlayLeftLabel.Text = playsLeft.ToString();

        if (DiscardButton != null) DiscardButton.Disabled = discardsLeft <= 0;
        if (PlayButton != null) PlayButton.Disabled = playsLeft <= 0;
    }

    // (com ordem decrescente dentro de cada naipe)
    private void SortBySuit()
    {
        var suitOrder = new Dictionary<Suit, int>
        {
            { Suit.Clubs, 0 },
            { Suit.Hearts, 1 },
            { Suit.Spades, 2 },
            { Suit.Diamonds, 3 }
        };

        _hand = _hand
            .OrderBy(c => suitOrder[c.Data.Suit])         // prioridade pelo naipe
            .ThenByDescending(c => (int)c.Data.Rank)      // ordem decrescente dentro do naipe
            .ToList();

        UpdateHandVisuals();
    }

    // (AS maior → 2 menor, com tie-breaker por suit)
    private void SortByRank()
    {
        var suitOrder = new Dictionary<Suit, int>
        {
            { Suit.Clubs, 0 },
            { Suit.Hearts, 1 },
            { Suit.Spades, 2 },
            { Suit.Diamonds, 3 }
        };

        _hand = _hand
            .OrderByDescending(c => (int)c.Data.Rank)     // rank maior primeiro
            .ThenBy(c => suitOrder[c.Data.Suit])          // desempate pelo suit
            .ToList();

        UpdateHandVisuals();
    }

    private void UpdateDrawButtonState()
    {
        if (DrawButton == null) return;

        DrawButton.Disabled = _hand.Count >= MaxHandSize || _deck.Count == 0;
    }
}
