using Godot;
using System;

public partial class GameOverScreen : Control
{
    [Export] private Button _menuButton;
    [Export] private Button _quitButton;
	[Export] private NodePath FinalScoreLabelPath;

    private Label _finalScoreLabel;
    private int? _injectedScore;

    public override void _Ready()
	{
		_finalScoreLabel = GetNodeOrNull<Label>(FinalScoreLabelPath);

        if (_menuButton != null) _menuButton.Pressed += OnMenuButtonPressed;
        else GD.PrintErr("GameOverScreen: Botão de Menu não foi atribuído no Inspetor.");

        if (_quitButton != null) _quitButton.Pressed += OnQuitButtonPressed;
        else GD.PrintErr("GameOverScreen: Botão de Sair não foi atribuído no Inspetor.");

        _finalScoreLabel = GetNodeOrNull<Label>(FinalScoreLabelPath);
        if (_finalScoreLabel == null)
            GD.PrintErr("GameOverScreen: FinalScoreLabelPath não aponta para uma Label válida.");

        int scoreToShow = 0;

        if (_injectedScore.HasValue)
        {
            scoreToShow = _injectedScore.Value;
        }
        else
        {
            var scene = GetTree().CurrentScene;
            var gm = scene?.GetNodeOrNull<GameManager>("GameManager");
            if (gm != null && GodotObject.IsInstanceValid(gm))
                scoreToShow = gm.BestRoundScore;
        }

        ApplyScore(scoreToShow);
    }

    public void SetFinalScore(int score)
    {
        _injectedScore = score;
        ApplyScore(score);
    }

    private void ApplyScore(int score)
    {
        if (_finalScoreLabel != null && GodotObject.IsInstanceValid(_finalScoreLabel))
            _finalScoreLabel.Text = score.ToString();
    }

    private void OnMenuButtonPressed()
    {
        var err = GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
        if (err != Error.Ok)
            GD.PrintErr($"Falha ao carregar o menu principal: {err}");
    }

    private void OnQuitButtonPressed()
    {
        var err = GetTree().ChangeSceneToFile("res://Scenes/home_screen.tscn");
    }
}