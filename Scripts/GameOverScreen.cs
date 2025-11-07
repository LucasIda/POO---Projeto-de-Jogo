using Godot;
using System;

public partial class GameOverScreen : Control
{
	[Export] private Button _menuButton;
	[Export] private Button _quitButton;

	public override void _Ready()
	{
		if (_menuButton != null)
		{
			_menuButton.Pressed += OnMenuButtonPressed;
		}
		else
		{
			GD.PrintErr("GameOverScreen: Botão de Menu não foi atribuído no Inspetor.");
		}

		if (_quitButton != null)
		{
			_quitButton.Pressed += OnQuitButtonPressed;
		}
		else
		{
			GD.PrintErr("GameOverScreen: Botão de Sair não foi atribuído no Inspetor.");
		}
	}

	private void OnMenuButtonPressed()
	{
		var err = GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
		if (err != Error.Ok)
		{
			GD.PrintErr($"Falha ao carregar o menu principal: {err}");
		}
	}

	private void OnQuitButtonPressed()
	{
		var err = GetTree().ChangeSceneToFile("res://Scenes/home_screen.tscn");
	}
}
