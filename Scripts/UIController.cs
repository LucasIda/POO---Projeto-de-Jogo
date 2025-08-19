using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class UIController : Control
{
    [Export] private PackedScene CardScene;
    [Export] private NodePath CardContainerPath;

    private Control _cardContainer;
    private List<string> _deck = new();
    private List<string> _discard = new();  // Cartas que foram retiradas (poderiam estar no "lixo" ou na mão)

    public override void _Ready()
    {
        _cardContainer = GetNode<Control>(CardContainerPath);

        // Carregar todas as cartas da pasta Sprites
        var dir = DirAccess.Open("res://Sprites/Cartas");
        if (dir != null)
        {
            dir.ListDirBegin();
            string fileName = dir.GetNext();
            while (fileName != "")
            {
                if (fileName.EndsWith(".png"))
                    _deck.Add(fileName);

                fileName = dir.GetNext();
            }
            dir.ListDirEnd();
        }

        // Embaralhar deck
        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        // Conectar os botões
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

        string cardName = _deck[0];
        _deck.RemoveAt(0);
        _discard.Add(cardName);  // Adiciona a carta ao "lixo" ou mão

        // Instanciar a cena Card
        var card = CardScene.Instantiate<Card>();
        Texture2D texture = GD.Load<Texture2D>($"res://Sprites/Cartas/{cardName}");
        card.SetCard(cardName, texture);

        // Conectar o evento de clique da carta
        card.OnCardClicked += OnCardClicked;

        // Calcular a posição da carta na mão
        int cardPositionInHand = _discard.Count;  // A posição da carta na mão é o número de cartas já retiradas
        GD.Print($"Carta sacada: {cardName}, Posição na mão: {cardPositionInHand}");

        // Definir posição da carta no container (para exibição)
        float cardOffsetX = _cardContainer.GetChildCount() * 110; // Calcula o deslocamento no eixo X
        card.Position = new Vector2(cardOffsetX, 0); // A posição é definida no eixo X

        _cardContainer.AddChild(card);
    }

    // Método para lidar com o clique na carta
    private void OnCardClicked(Card clickedCard)
    {
        GD.Print($"Carta {clickedCard.CardName} foi clicada!");

        // Alterna a seleção da carta clicada
        clickedCard.ToggleSelection();
    }

    private void OnReturnPressed()
    {
        if (_discard.Count == 0)
        {
            GD.Print("Não há cartas para devolver.");
            return;
        }

        // Remove a última carta da "mão"
        string lastCard = _discard.Last();
        _discard.RemoveAt(_discard.Count - 1);  // Remove a última carta do "lixo" ou mão

        // Recoloca a carta no deck (no começo da lista)
        _deck.Insert(0, lastCard);

        GD.Print($"Devolveu: {lastCard}");

        // Remover a carta do container (a última adicionada) e reorganizar
        if (_cardContainer.GetChildCount() > 0)
        {
            Node lastChild = _cardContainer.GetChild(_cardContainer.GetChildCount() - 1);
            lastChild.QueueFree(); // Libera o recurso da última carta

            // Reposiciona as cartas restantes para manter o visual
            for (int i = 0; i < _cardContainer.GetChildCount(); i++)
            {
                var card = _cardContainer.GetChild(i) as Card;
                if (card != null)
                {
                    card.Position = new Vector2(i * 110, 0);  // Reposiciona as cartas restantes
                }
            }
        }
    }

    private void OnResetPressed()
    {
        _deck.AddRange(_discard);
        _discard.Clear();

        // Embaralha o deck de novo
        GD.Randomize();
        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        GD.Print("Baralho resetado!");
        ClearCardContainer(); // Limpa o container de cartas
        ReorganizarCartas();  // Reorganiza as cartas no container após o reset
    }

    // Limpa o container de cartas
    private void ClearCardContainer()
    {
        foreach (Node child in _cardContainer.GetChildren())
        {
            child.QueueFree();  // Libera o recurso de cada carta
        }
    }

    // Reorganiza as cartas dentro do container após o reset
    private void ReorganizarCartas()
    {
        for (int i = 0; i < _cardContainer.GetChildCount(); i++)
        {
            var card = _cardContainer.GetChild(i) as Card;
            if (card != null)
            {
                card.Position = new Vector2(i * 110, 0);  // Reposiciona no eixo X
            }
        }
    }
}
