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
		_playerInventory = playerInventory;
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

		var gm = GetParent<GameManager>();
		if (gm != null)
		{
			gm.OnPlayerInventoryChanged += OnPlayerInventoryChangedFromGM;
		}


		await ToSignal(GetTree(), "process_frame");

		PopulateShop();
		UpdateRerollCostLabel();

		UpdateShopJokerState();
		UpdateTotalCostLabel();
	}

	private void PopulateShop(HashSet<string> excludeNames = null)
	{
		// devolve o que estava em exibição para a pool, para não “sumir”
		ReturnDisplayToPool();

		// limpa os filhos do container (se ainda houver)
		foreach (Node child in _jokerListContainer.GetChildren().ToList())
		{
			_jokerListContainer.RemoveChild(child);
		}

		// 1) Base de candidatos: não pode mostrar o que o jogador já possui
		var candidates = _shopMasterPool
			.Where(j => !_playerInventory.Any(owned => owned.Name == j.Name));

		// 2) Se for reroll, exclui os nomes que estavam exibidos antes
		if (excludeNames != null && excludeNames.Count > 0)
			candidates = candidates.Where(j => !excludeNames.Contains(j.Name));

		// 3) Seleciona aleatório
		var poolFiltered = candidates.OrderBy(_ => GD.Randi()).ToList();

		// 4) Pega o que der respeitando o limite da vitrine
		int needed = ShopDisplayCount;
		var chosen = new List<JokerCard>();

		int take = Math.Min(needed, poolFiltered.Count);
		chosen.AddRange(poolFiltered.Take(take));

		// 5) Fallback: se não tinha cartas suficientes “novas”,
		//    completa com o restante da pool (sempre sem repetir inventário)
		if (chosen.Count < needed)
		{
			var fallback = _shopMasterPool
				.Where(j =>
					!_playerInventory.Any(owned => owned.Name == j.Name) &&
					!chosen.Contains(j))
				.OrderBy(_ => GD.Randi())
				.Take(needed - chosen.Count);

			chosen.AddRange(fallback);
		}

		_currentDisplay = chosen;

		// 6) Move para a vitrine
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
		
		// 1) Captura os nomes que estavam na vitrine (para excluir no próximo sorteio)
		var excludeNames = _currentDisplay
			.Where(j => j != null && GodotObject.IsInstanceValid(j))
			.Select(j => j.Name)
			.ToHashSet();

		// 2) Devolve os exibidos para a pool (você já tinha essa lógica)
		foreach (var joker in _currentDisplay)
		{
			_shopMasterPool.Add(joker);
		}

		GD.Print("Loja atualizada (reroll).");

		// 3) Repreenche EXCLUINDO os anteriores
		PopulateShop(excludeNames);
	}

	private void UpdateShopJokerState()
	{
		bool isFull = _playerInventory.Count >= MaxJokerSlots;

		foreach (var joker in _currentDisplay.ToList())
		{
			if (joker == null || !GodotObject.IsInstanceValid(joker)) continue;

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
		// pode ser chamado numa janela de tempo em que a cena já saiu
		if (ShopCost == null || !GodotObject.IsInstanceValid(ShopCost)) return;

		int totalCost = _currentDisplay.Where(j => j.IsSelected).Sum(j => j.Cost);
		ShopCost.Text = totalCost > 0 ? $"{totalCost}" : "0";
	}

	private void UpdateRerollCostLabel()
	{
		if (RerollCostLabel == null || !GodotObject.IsInstanceValid(RerollCostLabel)) return;
		RerollCostLabel.Text = $"{_currentRerollCost}";
	}

	public void AddToPool(JokerCard joker)
	{
		if (joker == null || !GodotObject.IsInstanceValid(joker))
			return;

		// Garante estado “neutro” na loja
		joker.OnCardClicked -= OnJokerClicked;
		joker.MouseFilter = MouseFilterEnum.Stop;
		joker.Modulate = new Color(1f, 1f, 1f);
		joker.IsDraggable = false;
		if (joker.IsSelected) joker.ToggleSelection();

		// Evita duplicidade por Name (ou use referência)
		if (!_shopMasterPool.Any(j => j == joker || j.Name == joker.Name))
			_shopMasterPool.Add(joker);

		UpdateShopJokerState();
    	UpdateTotalCostLabel();
	}

	private void OnPlayerInventoryChangedFromGM()
	{
		UpdateShopJokerState();
		UpdateTotalCostLabel();
	}

	public override void _ExitTree()
	{
		var gm = GetParent<GameManager>();
		if (gm != null)
		{
			gm.OnPlayerInventoryChanged -= OnPlayerInventoryChangedFromGM;
		}
	}
	private void ReturnDisplayToPool()
	{
		foreach (var j in _currentDisplay.ToList())
		{
			// desconecta handlers
			j.OnCardClicked -= OnJokerClicked;

			// se ainda estiver na UI, remove
			if (j.GetParent() != null && GodotObject.IsInstanceValid(j.GetParent()))
				j.GetParent().RemoveChild(j);

			// volta para a pool (evita duplicidade)
			if (!_shopMasterPool.Contains(j))
				_shopMasterPool.Add(j);
		}
		_currentDisplay.Clear();
	}
}
