using Godot;
using System;

public partial class GameManager : Control
{
    [Export] private NodePath ChipsLabelPath;
    [Export] private NodePath AnteLabelPath;
    [Export] private PackedScene LojaScene;

    private Label _chipsLabel;
    private Label _anteLabel;

    private int _currentAnte = 0;   // Índice de ante (0 = Ante 1, 1 = Ante 2...)
    private int _currentBlind = 0;  // Índice de blind (0 = Small, 1 = Big, 2 = Boss)

    private int _requiredChips;
    private int _currentChips;

    public event Action OnRoundAdvanced;
    private Control _lojaInstance;


    private readonly int[,] AnteTable = new int[,]
    {
        { 300, 450, 600 },        // Ante 1: Small, Big, Boss
        { 800, 900, 1000 },       // Ante 2
        { 2000, 3000, 4000 },     // Ante 3
        { 6000, 9000, 12000 },    // Ante 4
        { 11000, 16500, 22000 },  // Ante 5
        { 20000, 30000, 40000 },  // Ante 6
        { 35000, 52500, 70000 },  // Ante 7
        { 50000, 75000, 100000 }, // Ante 8
    };

    private readonly string[] BlindNames = { "Small", "Big", "Boss" };

    public override void _Ready()
    {
        _chipsLabel = GetNode<Label>(ChipsLabelPath);
        _anteLabel = GetNode<Label>(AnteLabelPath);

        StartRound();
    }

    private void StartRound()
    {
        _currentChips = 0;
        _requiredChips = AnteTable[_currentAnte, _currentBlind];

        if (_chipsLabel != null)
            _chipsLabel.Text = $"Chips necessários = {_requiredChips}";

        if (_anteLabel != null)
            _anteLabel.Text = $"Ante {_currentAnte + 1} - {BlindNames[_currentBlind]}";

        GD.Print($"🔹 Iniciando {_anteLabel.Text}, meta = {_requiredChips}");
    }

    public void AddChips(int amount)
    {
        _currentChips += amount;
        GD.Print($"Chips atuais: {_currentChips}/{_requiredChips}");

        if (_currentChips >= _requiredChips)
        {
            GD.Print("🎉 Parabéns, você completou a meta!");
            MostrarLoja();
        }
    }

    private void NextRound()
    {
        _currentBlind++;

        if (_currentBlind >= 3) // passou de Boss
        {
            _currentBlind = 0;
            _currentAnte++;

            if (_currentAnte >= AnteTable.GetLength(0))
            {
                GD.Print("🏆 Você completou todas as antes!");
                return;
            }
        }

        OnRoundAdvanced?.Invoke();
    }

    public void ResetGlobalChips()
    {
        _currentChips = 0;
        //_currentAnte = 0; reseta antes
        //_currentBlind = 0; reseta blinds
        StartRound(); // reinicia o ante e atualiza labels
        GD.Print("⚠️ Pontuação global resetada!");
    }

    public int GetCurrentChips()
    {
        return _currentChips;
    }

    private void MostrarLoja()
    {
        if (LojaScene == null)
        {
            GD.PrintErr("⚠️ LojaScene não atribuída no GameManager!");
            StartRound();
            return;
        }

        // Instancia a loja
        _lojaInstance = LojaScene.Instantiate<Control>();
        AddChild(_lojaInstance); // ← adiciona sobre a cena principal

        // Procura botão "Seguir"
        var seguirBtn = _lojaInstance.GetNodeOrNull<Button>("PanelContainer/HBoxContainer/PassButton");
        if (seguirBtn != null)
        {
            seguirBtn.Pressed += () =>
            {
                NextRound(); // inicia próxima fase normalmente
                _lojaInstance.QueueFree(); // fecha o modal
            };
        }

        GD.Print("🛍️ Loja exibida sobre o jogo.");
    }
}