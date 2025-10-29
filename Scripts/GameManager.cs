using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
public partial class GameManager : Control
{
    [Export] private NodePath ChipsLabelPath;
    [Export] private NodePath AnteLabelPath;
    [Export] private PackedScene LojaScene;
    [Export] private PackedScene JokerScene;
    [Export] private Label PlayerCoin;
    [Export] private PackedScene GameOverScreen;

    private Label _chipsLabel;
    private Label _anteLabel;

    private int _currentAnte = 0;   // Índice de ante (0 = Ante 1, 1 = Ante 2...)
    private int _currentBlind = 0;  // Índice de blind (0 = Small, 1 = Big, 2 = Boss)
    public int PlayerCoins { get; private set; } = 4;

    private int _requiredChips;
    private int _currentChips;

    public event Action OnRoundAdvanced;
    private Control _lojaInstance;
    private Control _gameOverInstance;

    public List<JokerCard> MasterJokerPool { get; private set; } = new();
    
    public List<JokerCard> PlayerJokerInventory { get; private set; } = new();

    public event Action OnPlayerInventoryChanged;

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

        MasterJokerPool = JokerFactory.CreateJokers(JokerScene);
        GD.Print($"GameManager: Criados {MasterJokerPool.Count} curingas para o MasterPool.");

        PlayerCoin.Text = $"$ {PlayerCoins.ToString()}";

        StartRound();
    }

    private void StartRound()
    {
        _currentChips = 0;
        _requiredChips = AnteTable[_currentAnte, _currentBlind];

        if (_chipsLabel != null)
            _chipsLabel.Text = $"{_requiredChips}";

        if (_anteLabel != null)
            _anteLabel.Text = $"{_currentAnte + 1} {BlindNames[_currentBlind]}";

        GD.Print($"🔹 Iniciando {_anteLabel.Text}, meta = {_requiredChips}");
    }

    public void AddChips(int amount)
    {
        _currentChips += amount;
        GD.Print($"Chips atuais: {_currentChips}/{_requiredChips}");

        bool shopIsOpen = (_lojaInstance != null && IsInstanceValid(_lojaInstance));

        if (_currentChips >= _requiredChips && !shopIsOpen)
        {
            GD.Print("🎉 Parabéns, você completou a meta!");
            MostrarLoja();
        }
    }
    private void CalculateEndOfRoundBonuses()
    {
        GD.Print("--- Calculando Bônus de Fim de Rodada ---");
        GD.Print($"Moedas Iniciais: {PlayerCoins}");

        var uiController = GetParent<UIController>();
        if (uiController == null)
        {
            GD.PrintErr("GameManager não conseguiu encontrar o UIController para calcular bônus.");
            return;
        }

        int interestBonus = PlayerCoins / 5;
        AddCoins(interestBonus);
        GD.Print($"Bônus de Juros (1 por 5): +{interestBonus} moedas");

        int playsLeft = uiController.GetPlaysLeft();
        AddCoins(playsLeft);
        GD.Print($"Bônus de Jogadas Restantes: +{playsLeft} moedas");

        int blindBonus = 0;
        switch (_currentBlind)
        {
            case 0: // Small Blind
                blindBonus = 4;
                break;
            case 1: // Big Blind
                blindBonus = 5;
                break;
            case 2: // Boss Blind
                blindBonus = 6;
                break;
        }
        AddCoins(blindBonus);
        GD.Print($"Bônus do Blind ({BlindNames[_currentBlind]}): +{blindBonus} moedas");
        
        GD.Print($"Total de Moedas Final: {PlayerCoins}");
        GD.Print("------------------------------------------");

        PlayerCoin.Text = $"$ {PlayerCoins.ToString()}";
    }

    public void AddCoins(int amount)
    {
        if (amount > 0)
        {
            PlayerCoins += amount;
        }
    }

    private void NextRound()
    {
        _currentBlind++;

        if (_currentBlind >= 3)
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
        StartRound();
        GD.Print("⚠️ Pontuação global resetada!");
    }

    public int GetCurrentChips()
    {
        return _currentChips;
    }

    private void MostrarLoja()
    {
        if (_lojaInstance != null && IsInstanceValid(_lojaInstance))
        {
            GD.Print("Loja já está visível.");
            return;
        }

        CalculateEndOfRoundBonuses();

        if (LojaScene == null)
        {
            GD.PrintErr("⚠️ LojaScene não atribuída no GameManager!");
            StartRound();
            return;
        }

        _lojaInstance = LojaScene.Instantiate<Control>();

        AddChild(_lojaInstance); 

        var shopController = _lojaInstance as ShopController;
        if (shopController != null)
        {
            shopController.Initialize(MasterJokerPool, PlayerJokerInventory);
        }
        else
        {
            GD.PrintErr("A cena da Loja (loja.tscn) não tem o script ShopController.cs anexado ao seu nó raiz.");
        }

        var seguirBtn = _lojaInstance.GetNodeOrNull<Button>("PanelContainer/HBoxContainer/PassButton");
        if (seguirBtn != null)
        {
            seguirBtn.Pressed += () =>
            {
                if (shopController != null)
                {
                    MasterJokerPool = shopController.GetUpdatedMasterPool();
                    PlayerJokerInventory = shopController.GetUpdatedInventory();
                    OnPlayerInventoryChanged?.Invoke();
                    GD.Print($"Loja fechada. Inventário: {PlayerJokerInventory.Count}, Pool: {MasterJokerPool.Count}");
                }

                NextRound();
                _lojaInstance.QueueFree();
                _lojaInstance = null;
            };
        }

        GD.Print("🛍️ Loja exibida sobre o jogo.");
    }

    public void SetPlayerJokerOrder(List<JokerCard> newOrder)
    {
        PlayerJokerInventory = newOrder;
    }

    public void SpendCoins(int amount)
    {
        if (amount > PlayerCoins)
        {
            GD.PrintErr($"Tentativa de gastar {amount} moedas, mas o jogador só tem {PlayerCoins}. O gasto foi bloqueado.");
            return;
        }
        if (amount < 0) return;

        PlayerCoins -= amount;
        GD.Print($"Gastou {amount} moedas. Saldo restante: {PlayerCoins}");

        PlayerCoin.Text = $"$ {PlayerCoins.ToString()}";
    }
    
    public void CheckRoundEndState()
    {
        bool shopIsOpen = (_lojaInstance != null && IsInstanceValid(_lojaInstance));
        if (shopIsOpen)
        {
            return;
        }

bool gameOverIsOpen = (_gameOverInstance != null && IsInstanceValid(_gameOverInstance));
        if (gameOverIsOpen)
        {
            return;
        }

        if (_currentChips < _requiredChips)
        {
            GD.Print($"GAME OVER: Meta não atingida. Tinha {_currentChips} de {_requiredChips} necessários.");
            
            if (GameOverScreen == null)
            {
                GD.PrintErr("⚠️ GameOverScreen não atribuída no GameManager! Carregando cena de morte diretamente como fallback.");
                GetTree().ChangeSceneToFile("res://Scenes/morte.tscn");
                return;
            }

            _gameOverInstance = GameOverScreen.Instantiate<Control>();
            AddChild(_gameOverInstance);
            GD.Print("Tela de Game Over exibida.");
        }
    }
}