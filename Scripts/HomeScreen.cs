using Godot;

public partial class HomeScreen : Control
{
    private Button _startBtn, _rankingBtn, _quitBtn;
    private MusicManager _musicManager;  

    public override void _Ready()
    {
        _startBtn   = GetNodeOrNull<Button>("MarginContainer/HBoxContainer/VBoxContainer/start_btn");
        _rankingBtn = GetNodeOrNull<Button>("MarginContainer/HBoxContainer/VBoxContainer/ranking_btn");
        _quitBtn    = GetNodeOrNull<Button>("MarginContainer/HBoxContainer/VBoxContainer/quit_btn");
        _musicManager = GetNodeOrNull<MusicManager>("MusicManager");  // Referência ao MusicManager

        if (_startBtn == null || _rankingBtn == null || _quitBtn == null)
        {
            GD.PushError("Caminhos dos botões ou do MusicManager estão incorretos/vazios. Revise os nomes e hierarquia.");
            return; // Evita NullReference
        }

        _startBtn.Pressed   += OnStartBtnPressed;
        _rankingBtn.Pressed += OnRankingBtnPressed;
        _quitBtn.Pressed    += OnQuitBtnPressed;
    }

    private void OnStartBtnPressed()
    {
        var err = GetTree().ChangeSceneToFile("res://Scenes/main.tscn");
        if (err != Error.Ok) GD.PushError($"Falha ao trocar de cena: {err}");
    }

    private void OnRankingBtnPressed() 
    { 
        // Coloque a lógica para abrir o ranking quando existir
    }
    private void OnQuitBtnPressed() => GetTree().Quit();
}
