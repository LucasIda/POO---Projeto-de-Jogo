using Godot;

public partial class BuyArea : Panel
{
	[Export] public NodePath ShopPath; // arraste o nó Shop aqui
	private Shop _shop;

	public override void _Ready() => _shop = GetNode<Shop>(ShopPath);

	public override bool _CanDropData(Vector2 atPosition, Variant data)
		=> data.As<ShopItemView>() != null;

	public override void _DropData(Vector2 atPosition, Variant data)
	{
		var view = data.As<ShopItemView>();
		if (view == null || _shop == null) return;
		_shop.TryBuy(view);
	}
}
