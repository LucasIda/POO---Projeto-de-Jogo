using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShopController : Control
{
	[Export] private NodePath JokerListContainerPath;
	[Export] private NodePath BuyButtonPath;
	[Export] private NodePath RerollButtonPath;
	[Export] private Label ShopCost;
	[Export] private Label RerollCostLabel;
	
	private const int ShopDisplayCount = 3;
	private const int MaxJokerSlots = 5;
	private int _currentRerollCost = 2;

	private HBoxContainer _jokerListContainer;
	private Button _buyButton;
	private Button _rerollButton;

	private List<JokerCard> _shopMasterPool = new();
	private List<JokerCard> _playerInventory = new();
	private List<JokerCard> _currentDisplay = new();

	public void Initialize(List<JokerCard> masterPool, List<JokerCard> playerInventory)
	{
		_shopMasterPool = new List<JokerCard>(masterPool);
		_playerInventory = new List<JokerCard>(playerInventory);
	}

	public override async void _Ready()
	{
		_jokerListContainer = GetNode<HBoxContainer>(JokerListContainerPath);
		_buyButton = GetNode<Button>(BuyButtonPath);
		_rerollButton = GetNode<Button>(RerollButtonPath);

		if (_jokerListContainer == null || _buyButton == null || _rerollButton == null)
		{
			GD.PrintErr("ShopController: Um ou mais NodePaths não foram configurados no Inspetor.");
			return;
		}

		_buyButton.Pressed += OnBuyPressed;
		_rerollButton.Pressed += OnRerollPressed;

		await ToSignal(GetTree(), "process_frame");

		PopulateShop();
		UpdateRerollCostLabel();
	}

	private void PopulateShop()
	{
		foreach (Node child in _jokerListContainer.GetChildren().ToList()) //
		{
			_jokerListContainer.RemoveChild(child);

			if (child is JokerCard joker)
			{
				joker.OnCardClicked -= OnJokerClicked;
				if (joker.IsSelected) joker.ToggleSelection();
			}
		}
		_currentDisplay.Clear();

		var availablePool = _shopMasterPool
			.Where(joker => !_playerInventory.Any(owned => owned.Name == joker.Name))
			.ToList();
		availablePool = availablePool.OrderBy(x => GD.Randi()).ToList();
		int count = Math.Min(availablePool.Count, ShopDisplayCount);
		_currentDisplay = availablePool.Take(count).ToList();

		foreach (var joker in _currentDisplay)
		{
			_shopMasterPool.Remove(joker); 
			_jokerListContainer.AddChild(joker);
			joker.OnCardClicked += OnJokerClicked;

			joker.IsDraggable = false;

			joker.TooltipDisplayDirection = TooltipDirection.Above;
		}

		UpdateShopJokerState();
		UpdateTotalCostLabel();
	}

	private void OnJokerClicked(BaseCard clickedCard)
	{
		if (!clickedCard.IsSelected && _playerInventory.Count >= MaxJokerSlots)
		{
			GD.Print("Inventário de Curingas cheio! (5/5)");
			return;
		}

		clickedCard.ToggleSelection();
		
		UpdateTotalCostLabel();
	}

	private void OnBuyPressed()
	{
		var boughtJokers = _currentDisplay.Where(j => j.IsSelected).ToList(); //
		
		if (boughtJokers.Count == 0)
		{
			GD.Print("Nenhum curinga selecionado para compra.");
			return;
		}
		
		if (_playerInventory.Count + boughtJokers.Count > MaxJokerSlots)
		{
			GD.Print($"ERRO: Você não pode comprar! Você tem {_playerInventory.Count}/5 e está tentando comprar {boughtJokers.Count}.");
			return;
		}

		var gameManager = GetParent<GameManager>();
		if (gameManager == null)
		{
			GD.PrintErr("ShopController não conseguiu encontrar o GameManager!");
			return;
		}

		int totalCost = boughtJokers.Sum(joker => joker.Cost);
		
		if (gameManager.PlayerCoins < totalCost)
		{
			GD.Print($"Moedas insuficientes! Você tem {gameManager.PlayerCoins}, mas precisa de {totalCost}.");
			return;
		}

		gameManager.SpendCoins(totalCost);

		foreach (var joker in boughtJokers)
		{
			GD.Print($"Jogador comprou {joker.Name} por {joker.Cost} moedas.");

			_playerInventory.Add(joker);
			_currentDisplay.Remove(joker);

			joker.OnCardClicked -= OnJokerClicked;
			if (joker.IsSelected) joker.ToggleSelection();

			joker.IsDraggable = true;

			_jokerListContainer.RemoveChild(joker);
		}

		UpdateShopJokerState();
		UpdateTotalCostLabel();
	}

	private void OnRerollPressed()
	{
		var gameManager = GetParent<GameManager>();
		if (gameManager == null)
		{
			GD.PrintErr("ShopController não conseguiu encontrar o GameManager!");
			return;
		}

		if (gameManager.PlayerCoins < _currentRerollCost)
		{
			GD.Print($"Moedas insuficientes para atualizar! Você tem {gameManager.PlayerCoins}, mas precisa de {_currentRerollCost}.");
			return;
		}

		gameManager.SpendCoins(_currentRerollCost);

		_currentRerollCost += 1;

		UpdateRerollCostLabel();
		
		foreach (var joker in _currentDisplay)
		{
			_shopMasterPool.Add(joker);
		}
		
		GD.Print("Loja atualizada (reroll).");
		PopulateShop();
	}

	private void UpdateShopJokerState()
	{
		bool isFull = _playerInventory.Count >= MaxJokerSlots;

		foreach (var joker in _currentDisplay)
		{
			if (isFull && !joker.IsSelected)
			{
				joker.MouseFilter = MouseFilterEnum.Ignore;
				joker.Modulate = new Color(0.5f, 0.5f, 0.5f); 
			}
			else
			{
				joker.MouseFilter = MouseFilterEnum.Stop; 
				joker.Modulate = new Color(1f, 1f, 1f);
			}
		}
	}

	public List<JokerCard> GetUpdatedMasterPool()
	{
		foreach (var joker in _currentDisplay)
		{
			joker.OnCardClicked -= OnJokerClicked; //

			if (joker.GetParent() != null)
			{
				joker.GetParent().RemoveChild(joker); //
			}
			_shopMasterPool.Add(joker); //
		}
		
		_shopMasterPool = _shopMasterPool.OrderBy(x => GD.Randi()).ToList();
		return _shopMasterPool;
	}

	public List<JokerCard> GetUpdatedInventory()
	{
		return _playerInventory;
	}

	private void UpdateTotalCostLabel()
	{
		var selectedJokers = _currentDisplay.Where(j => j.IsSelected).ToList();

		int totalCost = selectedJokers.Sum(joker => joker.Cost);

		if (ShopCost != null)
		{
			if (totalCost > 0)
			{
				ShopCost.Text = $"{totalCost}";
			}
			else
			{
				ShopCost.Text = $"0";
			}
		}
	}
	
	private void UpdateRerollCostLabel()
	{
		if (RerollCostLabel != null)
		{
			// Exibe o custo ao lado do botão "Atualizar"
			RerollCostLabel.Text = $"{_currentRerollCost}";
		}
	}
}
