using Godot;
using System;

public partial class Card : TextureRect
{
    public string CardName { get; private set; }

    public bool IsSelected { get; private set; }  // Estado de seleção da carta

    // Sinal para quando a carta for clicada
    public delegate void CardClicked(Card clickedCard);
    public event CardClicked OnCardClicked;

    public void SetCard(string name, Texture2D texture)
    {
        CardName = name;
        Texture = texture;
        
        // Conectar o sinal de clique do controle
        Connect("gui_input", new Callable(this, "OnCardInput"));
    }

    // Detectar cliques na carta
    private void OnCardInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton mouseEvent &&
            mouseEvent.ButtonIndex == MouseButton.Left &&  // <- CORRETO para Godot 4
            mouseEvent.Pressed)
        {
            OnCardClicked?.Invoke(this);  // Dispara o evento de clique
        }
    }


    // Método para alternar o estado de seleção
    public void ToggleSelection()
    {
        IsSelected = !IsSelected;
        Modulate = IsSelected ? new Color(1, 1, 1, 0.5f) : new Color(1, 1, 1, 1);  // Altera a opacidade
    }
}
