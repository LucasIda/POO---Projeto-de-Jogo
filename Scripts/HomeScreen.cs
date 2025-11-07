using Godot;

public partial class HomeScreen : Control
{
    private Button _startBtn, _rankingBtn, _quitBtn;
    private MusicManager _musicManager;
    private Label _volumeLabel;
    private HSlider _volumeSlider;  


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

        _volumeSlider = GetNode<HSlider>("MarginContainer/HBoxContainer/VBoxContainer/VolumeContainer/VolumeSlider");
        _volumeLabel = GetNode<Label>("MarginContainer/HBoxContainer/VBoxContainer/VolumeContainer/VolumeLabel");

        // Carrega volume salvo
        var config = new ConfigFile();
        var err = config.Load("user://settings.cfg");
        float saved = 0.7f;
        if (err == Error.Ok)
        {
            var variant = config.GetValue("audio", "music_volume", 0.7f);
            saved = variant.AsSingle();
        }
        _volumeSlider.Value = saved;
        UpdateVolumeLabel(saved);

        _volumeSlider.ValueChanged += OnVolumeChanged;
        OnVolumeChanged(saved);
        
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

private void OnVolumeChanged(double value)
    {
        float norm = (float)value;
        float db = Mathf.Lerp(-30.0f, 0.0f, norm);

        GetNode<MusicManager>("/root/MusicManager").SetVolumeDb(db);
        UpdateVolumeLabel(norm);

        // Salva
        var config = new ConfigFile();
        config.SetValue("audio", "music_volume", norm);
        config.Save("user://settings.cfg");
    }

    private void UpdateVolumeLabel(float norm)
    {
        _volumeLabel.Text = $"Volume: {(int)(norm * 100)}%";
    }
}
