using System;
using System.Collections.Generic;

public class JokerData
{
    public string Name { get; private set; }
    public string TexturePath { get; private set; }
    public int Multiplier { get; private set; }

    public JokerData(string name, string texturePath, int multiplier)
    {
        Name = name;
        TexturePath = texturePath;
        Multiplier = multiplier;
    }
}

public static class JokerDatabase
{
    public static List<JokerData> GenerateJokers()
    {
        // Exemplo: você pode adicionar quantos curingas quiser
        return new List<JokerData>()
        {
            new JokerData("C_opcional1", "res://Sprites/Cartas/C_opcional1.png", 2),
            
        };
    }
}
