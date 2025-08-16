using Godot;
using System;

public partial class Card : TextureRect
{
    public string CardName { get; private set; }

    public void SetCard(string name, Texture2D texture)
    {
        CardName = name;
        Texture = texture;
    }
}
