using Godot;
using System;

public partial class Card : TextureRect
{
    public CardData Data { get; private set; }
    public bool IsSelected { get; private set; }

    public delegate void CardClicked(Card clickedCard);
    public event CardClicked OnCardClicked;

    public void SetCard(CardData data, Texture2D texture)
    {
        Data = data;
        Texture = texture;

        Connect("gui_input", new Callable(this, nameof(OnCardInput)));
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
}
