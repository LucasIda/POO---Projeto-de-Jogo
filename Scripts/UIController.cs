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

	//  Lista de cartas atualmente selecionadas
	private List<Card> _selectedCards = new();

	// >>> LOJA: estado básico
	public int Coins { get; set; } = 20;   // moedas usadas pela loja
	public int Level { get; set; } = 0;    // nível/round atual
	private bool _shopOpen = false;        // evita abrir duas lojas

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

		if (clickedCard.IsSelected)
		{
			if (!_selectedCards.Contains(clickedCard))
				_selectedCards.Add(clickedCard);
		}
		else
		{
			_selectedCards.Remove(clickedCard);
		}

		//  Chama o avaliador sempre que algo mudar
		if (_selectedCards.Count > 0)
		{
			var selectedData = _selectedCards.Select(c => c.Data).ToList();
			var hand = HandEvaluator.EvaluateHand(selectedData);
			GD.Print($"Mão atual: {hand}");
		}
		else
		{
			GD.Print("Nenhuma carta selecionada.");
		}
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

		//  Também limpa lista de selecionadas caso devolva uma que estava marcada
		_selectedCards.RemoveAll(c => !IsInstanceValid(c));
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
	}

	private void ClearCardContainer()
	{
		foreach (Node child in _cardContainer.GetChildren())
			child.QueueFree();
	}


	// Chame isto quando terminar o 1º round/nível
	public void OnRoundEnded()
	{
		Level++;
		if (Level >= 1 && !_shopOpen)
			OpenShop();
	}
	public override void _Input(InputEvent e)
	{
   		 if (e is InputEventKey k && k.Pressed && !k.Echo && k.Keycode == Key.L)
	{
		GD.Print("L pressionado");
		OnRoundEnded(); // abre a loja
	}
}

	// chamado pelo Shop quando fecha (Next Round)
	public void OnShopClosed()
	{
		_shopOpen = false;
		GD.Print("Shop fechada — seguir para a próxima rodada.");
		// TODO: retomar jogo, iniciar próxima rodada etc.
	}

	private void OpenShop()
	{
		var shopPacked = GD.Load<PackedScene>("res://Scenes/shop.tscn");
		var shop = shopPacked.Instantiate<Shop>(); // precisa do Shop.cs
		AddChild(shop);
		_shopOpen = true;

		// catálogo temporário (troque pelos itens reais que implementem IShopItem)
		var catalog = new List<IShopItem>
		{
			new DemoItem("Empress", "Carta especial", 3),
			new DemoItem("Joker", "Carta de joker", 2),
			new DemoItem("Buffoon Pack", "Pacote comum", 7),
			new DemoItem("Arcana Pack", "Pacote raro", 9),
			new DemoItem("Voucher", "Bônus único", 10),
		};

		shop.Open(this, catalog);
	}

	//  abra a loja ao iniciar, só para teste
	private void OpenShopForTest()
	{
		OnRoundEnded();
	}
}
