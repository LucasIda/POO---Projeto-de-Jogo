using Godot;

public class DemoItem : IShopItem
{
	public string DisplayName { get; }
	public string Description { get; }
	public int Price { get; }

	public DemoItem(string name, string desc, int price) // 
	{
		DisplayName = name;
		Description = desc;
		Price = price;
	}

	public void Apply(UIController game)
	{
		
		GD.Print($"[DemoItem] Aplicou efeito de: {DisplayName}");
	}
}
