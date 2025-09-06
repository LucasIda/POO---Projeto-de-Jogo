using Godot;
using System.Collections.Generic;

public partial class Shop : CanvasLayer
{
	[Export] public NodePath TopRowPath;
	[Export] public NodePath BottomRowPath;
	[Export] public NodePath CoinsPath;
	[Export] public NodePath NextRoundBtnPath;
	[Export] public NodePath RerollBtnPath;
	[Export] public PackedScene ItemViewScene; // arraste res://Scenes/ShopItemView.tscn
	[Export] public int RerollCost = 5;

	private GridContainer _topRow, _bottomRow;
	private Label _coins;
	private Button _nextBtn, _rerollBtn;

	public UIController Game;
	private List<IShopItem> _catalog = new();

	public override void _Ready()
	{
		_topRow    = GetNode<GridContainer>(TopRowPath);
		_bottomRow = GetNode<GridContainer>(BottomRowPath);
		_coins     = GetNode<Label>(CoinsPath);
		_nextBtn   = GetNode<Button>(NextRoundBtnPath);
		_rerollBtn = GetNode<Button>(RerollBtnPath);

		_nextBtn.Pressed  += OnNextRoundPressed;
		_rerollBtn.Pressed += OnRerollPressed;

		Hide(); // quem chama Open() é o controlador do jogo
	}

	// Abre a loja com catálogo
	public void Open(UIController gameRef, List<IShopItem> items)
	{
		Game = gameRef;
		_catalog = items ?? new List<IShopItem>();
		Populate();
		UpdateCoins();
		Show();
		GetTree().Paused = true;
	}

	private void Populate()
	{
		foreach (var c in _topRow.GetChildren()) c.QueueFree();
		foreach (var c in _bottomRow.GetChildren()) c.QueueFree();

		for (int i = 0; i < _catalog.Count; i++)
		{
			var view = ItemViewScene.Instantiate<ShopItemView>();
			(i < 2 ? _topRow : _bottomRow).AddChild(view);
			view.Bind(_catalog[i], null); // ícone entra depois
		}
	}

	// Chamado pela BuyArea quando solta um card nela
	public void TryBuy(ShopItemView view)
	{
		if (view == null || view.BoundItem == null) return;
		var item = view.BoundItem;

		if (Game != null && Game.Coins < item.Price)
		{
			GD.Print("Moedas insuficientes!");
			return;
		}

		if (Game != null) Game.Coins -= item.Price;
		item.Apply(Game);
		view.QueueFree(); // remove da vitrine
		UpdateCoins();
		GD.Print($"Comprou: {item.DisplayName} por ${item.Price}");
	}

	private void OnNextRoundPressed()
	{
		Close();
		Game?.OnShopClosed();
	}

	private void OnRerollPressed()
	{
		if (Game != null && Game.Coins < RerollCost)
		{
			GD.Print("Sem moedas para Reroll!");
			return;
		}

		if (Game != null) Game.Coins -= RerollCost;

		// TODO: gerar novo catálogo; provisório = inverter
		_catalog.Reverse();
		Populate();
		UpdateCoins();
	}

	public void Close()
	{
		Hide();
		GetTree().Paused = false;
		QueueFree();
	}

	private void UpdateCoins()
	{
		_coins.Text = $"Moedas: {Game?.Coins ?? 0}";
	}
}
