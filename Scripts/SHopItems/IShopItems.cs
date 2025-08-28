public interface IShopItem
{
	string TitleWithPrice();
	int Price { get; }
	void Apply(UIController game);
}
