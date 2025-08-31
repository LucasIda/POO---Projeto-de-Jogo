using Godot;
using System;

public partial class DeckView : Control
{
    [Export] private string CardBackPath = "res://Sprites/Verso/red_backing.png";
    [Export] private int MaxLayers = 13;
    [Export] private float LayerOffset = 2f;
    [Export] private float RotationOffset = 3f;

    private int _totalCards;
    private int _cardsRemaining;

    private Control _stackContainer; // Container para as sprites
    private Label _countLabel;
    private Texture2D _cardTexture;

    public override void _Ready()
    {
        // Carrega textura do verso
        _cardTexture = GD.Load<Texture2D>(CardBackPath);
        if (_cardTexture == null)
        {
            GD.PrintErr("Não foi possível carregar a textura do verso das cartas!");
            return;
        }

        // Cria container para a pilha
        _stackContainer = new Control();
        AddChild(_stackContainer);
        _stackContainer.SetZIndex(0);

        // Label para contagem, posicionada abaixo da pilha
        _countLabel = GetNodeOrNull<Label>("CardCountLabel");
        if (_countLabel == null)
        {
            _countLabel = new Label();
            _countLabel.Position = new Vector2(0, 60); // abaixo da pilha
            AddChild(_countLabel);
        }
        _countLabel.SetZIndex(1);

        // Inicializa deck
        if (_totalCards == 0) _totalCards = 52;
        _cardsRemaining = _totalCards;

        RedrawStack();
        UpdateLabel();
    }

    // Inicializa deck com total definido
    public void Initialize(int totalCards)
    {
        _totalCards = totalCards;
        _cardsRemaining = totalCards;
        RedrawStack();
        UpdateLabel();
    }

    // Atualiza quantidade atual e total
    public void UpdateCount(int currentCount, int totalCount)
    {
        _cardsRemaining = currentCount;
        _totalCards = totalCount;

        RedrawStack();
        UpdateLabel();
    }

    private void RedrawStack()
    {
        // Limpa sprites antigas
        foreach (Node child in _stackContainer.GetChildren())
        {
            if (child is Sprite2D) child.QueueFree();
        }

        if (_cardsRemaining <= 0) return;

        // Calcula o número de layers proporcional às cartas restantes
        int layers = Mathf.CeilToInt((_cardsRemaining / (float)_totalCards) * MaxLayers);
        layers = Mathf.Max(layers, 1); // garante pelo menos 1 layer visível

        float smoothOffset = LayerOffset / 2f;

        for (int i = 0; i < layers; i++)
        {
            float xOffset = i * LayerOffset/4;
            float yOffset = -i * LayerOffset/2;

            var sprite = new Sprite2D
            {
                Texture = _cardTexture,
                Position = new Vector2(xOffset, yOffset),
                RotationDegrees = 0
            };

            _stackContainer.AddChild(sprite);
        }
    }


    private void UpdateLabel()
    {
        if (_countLabel != null)
            _countLabel.Text = $"{_cardsRemaining}/{_totalCards}";
    }

    // Reset deck para o total definido
    public void ResetDeck(int totalCount)
    {
        _totalCards = totalCount;
        _cardsRemaining = totalCount;
        RedrawStack();
        UpdateLabel();
    }
}
