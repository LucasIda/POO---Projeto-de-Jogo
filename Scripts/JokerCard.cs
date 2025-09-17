using Godot;
using System;

public partial class JokerCard : BaseCard
{
    public int Multiplier { get; private set; }

    public void SetJoker(string name, Texture2D texture, int multiplier)
    {
        Multiplier = multiplier;
        Initialize(name, texture);
    }

    public void ActivateEffect()
    {
        GD.Print($"Curinga {Name} ativado! Multiplicador: {Multiplier}x");
        // Aqui você implementa o efeito real do curinga
    }
}
