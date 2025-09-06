public interface IShopItem
{
	string DisplayName { get; }
	string Description { get; }
	int Price { get; }
	void Apply(UIController game);
}
