using Godot;

public partial class ShopItemView : VBoxContainer
{
	[Export] public NodePath IconPath;
	[Export] public NodePath PricePath;

	private TextureRect _icon;
	private Label _price;

	public IShopItem BoundItem { get; private set; }

	public override void _Ready()
	{
		_icon  = GetNode<TextureRect>(IconPath);
		_price = GetNode<Label>(PricePath);
	}

	public void Bind(IShopItem item, Texture2D icon = null)
	{
		BoundItem   = item;
		_price.Text = $"${item.Price}";
		if (icon != null) _icon.Texture = icon;
		TooltipText = item.Description; // tooltip ao passar o mouse
	}

	// Permite arrastar o card (drag)
	public override Variant _GetDragData(Vector2 atPosition)
	{
		// dados do drag: a própria view
		var data = this;

		// monta um preview leve (um VBox com ícone + preço)
		var preview = new VBoxContainer();

		var iconCopy = new TextureRect
		{
			Texture = _icon?.Texture,
			StretchMode = TextureRect.StretchModeEnum.KeepAspectCentered,
			CustomMinimumSize = new Vector2(140, 160)
		};

		var priceCopy = new Label
		{
			Text = _price?.Text ?? "$0",
			HorizontalAlignment = HorizontalAlignment.Center
		};

		preview.AddChild(iconCopy);
		preview.AddChild(priceCopy);

		SetDragPreview(preview);   // é um Control → compila

		return data;
	}
}
