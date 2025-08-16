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
    private List<string> _discard = new();

    public override void _Ready()
    {
        _cardContainer = GetNode<Control>(CardContainerPath);

        // Carrega todas as cartas da pasta Sprites
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

        string cardName = _deck[0];
        _deck.RemoveAt(0);
        _discard.Add(cardName);

        // Instanciar cena Card
        var card = CardScene.Instantiate<Card>();
        Texture2D texture = GD.Load<Texture2D>($"res://Sprites/Cartas/{cardName}");
        card.SetCard(cardName, texture);

        _cardContainer.AddChild(card);

        GD.Print($"Sacou: {cardName}");
    }

private void OnReturnPressed()
{
    if (_discard.Count == 0) return;

    // Remove a última carta do deck e do container
    string lastCard = _discard.Last();
    _discard.RemoveAt(_discard.Count - 1);
    _deck.Insert(0, lastCard);

    GD.Print($"Devolveu: {lastCard}");

    // Remove apenas a última carta do CardContainer
    if (_cardContainer.GetChildCount() > 0)
    {
        Node lastChild = _cardContainer.GetChild(_cardContainer.GetChildCount() - 1);
        lastChild.QueueFree();
    }
}


    private void OnResetPressed()
    {
        _deck.AddRange(_discard);
        _discard.Clear();

        _deck = _deck.OrderBy(x => GD.Randi()).ToList();

        GD.Print("Baralho resetado!");
        ClearCardContainer();
    }

    // 🔹 Método para limpar o container de cartas
    private void ClearCardContainer()
    {
        foreach (Node child in _cardContainer.GetChildren())
        {
            child.QueueFree();
        }
    }
}
