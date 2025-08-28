using Godot;
using System.Collections.Generic;

public partial class Shop : CanvasLayer
{
	[Export] public NodePath ItemListPath;
	[Export] public NodePath BuyBtnPath;
	[Export] public NodePath CloseBtnPath;
	[Export] public NodePath CoinsLabelPath;

	public UIController Game;
	private ItemList _itemList;
	private Button _buyBtn, _closeBtn;
	private Label _coinsLabel;
	private List<IShopItem> _items = new();

	public override void _Ready()
	{
		_itemList = GetNode<ItemList>(ItemListPath);
		_buyBtn = GetNode<Button>(BuyBtnPath);
		_closeBtn = GetNode<Button>(CloseBtnPath);
		_coinsLabel = GetNode<Label>(CoinsLabelPath);

		_buyBtn.Pressed += OnBuyPressed;
		_closeBtn.Pressed += OnClosePressed;
	}

	public void Open(UIController gameRef, List<IShopItem> items)
	{
		Game = gameRef;
		_items = items ?? new List<IShopItem>();
		_itemList?.Clear();

		foreach (var it in _items)
			_itemList.AddItem(it.TitleWithPrice());

		UpdateCoinsLabel();
		Show();
		GetTree().Paused = true; // pausa o jogo por trás
	}

	private void OnBuyPressed()
	{
		if (_items.Count == 0) return;
		var idx = _itemList.GetSelectedItems();
		if (idx.Length == 0) return;

		var item = _items[idx[0]];
		if (Game.Coins >= item.Price)
		{
			Game.Coins -= item.Price;
			item.Apply(Game);
			UpdateCoinsLabel();
			GD.Print($"Comprou: {item.TitleWithPrice()}");
		}
		else
		{
			GD.Print("Moedas insuficientes!");
		}
	}

	private void OnClosePressed()
	{
		Hide();
		GetTree().Paused = false;
		Game?.OnShopClosed();
		QueueFree();
	}

	private void UpdateCoinsLabel()
	{
		if (_coinsLabel != null)
			_coinsLabel.Text = $"Moedas: {Game?.Coins ?? 0}";
	}
}
