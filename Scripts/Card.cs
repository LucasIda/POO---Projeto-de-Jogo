using Godot;
using System;

public partial class Card : TextureRect
{
	public CardData Data { get; private set; }
	public bool IsSelected { get; private set; }

	public delegate void CardClicked(Card clickedCard);
	public event CardClicked OnCardClicked;

	private PanelContainer tooltip;

	public void SetCard(CardData data, Texture2D texture)
	{
		Data = data;
		Texture = texture;

		// Conectar eventos de mouse
		Connect("gui_input", new Callable(this, nameof(OnCardInput)));
		MouseEntered += OnMouseEntered;
		MouseExited += OnMouseExited;
	}

	private void OnCardInput(InputEvent @event)
	{
		if (@event is InputEventMouseButton mouseEvent &&
			mouseEvent.ButtonIndex == MouseButton.Left &&
			mouseEvent.Pressed)
		{
			OnCardClicked?.Invoke(this);
			ShowInfo();
		}
	}

	public void ToggleSelection()
	{
		IsSelected = !IsSelected;
		Modulate = IsSelected
			? new Color(1, 1, 1, 0.5f)
			: new Color(1, 1, 1, 1);
	}

	public void ShowInfo()
	{
		GD.Print($"Nome completo: {Data.Name}");
		GD.Print($"Rank: {Data.Rank}");
		GD.Print($"Naipe: {Data.Suit}");
	}

	private void OnMouseEntered()
	{
		ShowTooltip();
	}

	private void OnMouseExited()
	{
		HideTooltip();
	}

	private void ShowTooltip()
	{
		if (tooltip != null) return;

		tooltip = new PanelContainer
		{
			Modulate = new Color(0, 0, 0, 0.85f),
			SizeFlagsHorizontal = Control.SizeFlags.ShrinkCenter,
			SizeFlagsVertical = Control.SizeFlags.ShrinkCenter
		};

		// Adiciona padding interno e bordas arredondadas
		tooltip.AddThemeConstantOverride("margin_left", 6);
		tooltip.AddThemeConstantOverride("margin_right", 6);
		tooltip.AddThemeConstantOverride("margin_top", 4);
		tooltip.AddThemeConstantOverride("margin_bottom", 4);
		tooltip.AddThemeColorOverride("border_color", new Color(1, 1, 1, 0.9f));
		tooltip.AddThemeConstantOverride("border_width", 2);
		tooltip.AddThemeConstantOverride("corner_radius", 6);

		// Caixa organizada
		var vbox = new VBoxContainer
		{
			CustomMinimumSize = new Vector2(120, 0)
		};

		// Define espaçamento entre linhas
		vbox.AddThemeConstantOverride("separation", 4);

		var nameLabel = new Label { Text = $"Nome: {Data.Name}" };
		var chipLabel = new Label { Text = $"Chips: {GetChipValue()}" };

		vbox.AddChild(nameLabel);
		vbox.AddChild(chipLabel);

		tooltip.AddChild(vbox);

		// Adiciona como filho da própria carta, para ficar em cima
		AddChild(tooltip);

		// Posição acima da carta
		tooltip.Position = new Vector2(0, -tooltip.Size.Y - 10);
	}

	private void HideTooltip()
	{
		tooltip?.QueueFree();
		tooltip = null;
	}

	private int GetChipValue()
	{
		return Data.Rank switch
		{
			Rank.Ace => 11,
			Rank.Ten or Rank.Jack or Rank.Queen or Rank.King => 10,
			_ => (int)Data.Rank
		};
	}



}
