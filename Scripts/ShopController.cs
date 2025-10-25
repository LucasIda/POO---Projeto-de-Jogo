using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShopController : Control
{
    [Export] private NodePath JokerListContainerPath;
    [Export] private NodePath BuyButtonPath;
    [Export] private NodePath RerollButtonPath;
    
    private const int ShopDisplayCount = 3;
    
    // --- 1. ADICIONE A CONSTANTE DO LIMITE ---
    private const int MaxJokerSlots = 5;

    private HBoxContainer _jokerListContainer;
    private Button _buyButton;
    private Button _rerollButton;

    private List<JokerCard> _shopMasterPool = new();
    private List<JokerCard> _playerInventory = new();
    private List<JokerCard> _currentDisplay = new();

    public void Initialize(List<JokerCard> masterPool, List<JokerCard> playerInventory)
    {
        _shopMasterPool = new List<JokerCard>(masterPool);
        _playerInventory = new List<JokerCard>(playerInventory);
    }

    public override async void _Ready()
    {
        _jokerListContainer = GetNode<HBoxContainer>(JokerListContainerPath);
        _buyButton = GetNode<Button>(BuyButtonPath);
        _rerollButton = GetNode<Button>(RerollButtonPath);

        if (_jokerListContainer == null || _buyButton == null || _rerollButton == null)
        {
            GD.PrintErr("ShopController: Um ou mais NodePaths não foram configurados no Inspetor.");
            return;
        }

        _buyButton.Pressed += OnBuyPressed;
        _rerollButton.Pressed += OnRerollPressed;

        await ToSignal(GetTree(), "process_frame");
        
        PopulateShop();
    }

    private void PopulateShop()
    {
        // 1. Limpa os filhos antigos
        foreach (Node child in _jokerListContainer.GetChildren().ToList()) //
        {
            _jokerListContainer.RemoveChild(child);

            if (child is JokerCard joker)
            {
                joker.OnCardClicked -= OnJokerClicked;
                if (joker.IsSelected) joker.ToggleSelection();
            }
        }
        _currentDisplay.Clear(); //

        // 2. Pega os curingas disponíveis
        var availablePool = _shopMasterPool
            .Where(joker => !_playerInventory.Any(owned => owned.Name == joker.Name))
            .ToList();
        availablePool = availablePool.OrderBy(x => GD.Randi()).ToList();
        int count = Math.Min(availablePool.Count, ShopDisplayCount);
        _currentDisplay = availablePool.Take(count).ToList();

        // 3. Adiciona APENAS os curingas
        foreach (var joker in _currentDisplay)
        {
            _shopMasterPool.Remove(joker); 
            _jokerListContainer.AddChild(joker);
            joker.OnCardClicked += OnJokerClicked; //
        }
        
        // --- 4. ATUALIZA A UI (NOVO) ---
        UpdateShopJokerState();
    }

    // --- 2. MODIFIQUE O OnJokerClicked ---
    private void OnJokerClicked(BaseCard clickedCard)
    {
        // Se o jogador já tem 5, ele não pode selecionar um novo.
        // (Ele ainda pode des-selecionar)
        if (!clickedCard.IsSelected && _playerInventory.Count >= MaxJokerSlots)
        {
            GD.Print("Inventário de Curingas cheio! (5/5)");
            return;
        }
        
        clickedCard.ToggleSelection(); //
    }

    // --- 3. MODIFIQUE O OnBuyPressed ---
    private void OnBuyPressed()
    {
        var boughtJokers = _currentDisplay.Where(j => j.IsSelected).ToList(); //
        
        if (boughtJokers.Count == 0)
        {
            GD.Print("Nenhum curinga selecionado para compra.");
            return;
        }
        
        // VERIFICAÇÃO DE LIMITE
        if (_playerInventory.Count + boughtJokers.Count > MaxJokerSlots)
        {
            GD.Print($"ERRO: Você não pode comprar! Você tem {_playerInventory.Count}/5 e está tentando comprar {boughtJokers.Count}.");
            return; // Bloqueia a compra
        }
        
        foreach (var joker in boughtJokers)
        {
            GD.Print($"Jogador comprou {joker.Name}.");
            
            _playerInventory.Add(joker); //
            _currentDisplay.Remove(joker); //
            
            joker.OnCardClicked -= OnJokerClicked; //
            if (joker.IsSelected) joker.ToggleSelection(); 

            _jokerListContainer.RemoveChild(joker); //
        }
        
        // ATUALIZA A UI (NOVO)
        UpdateShopJokerState();
    }

    private void OnRerollPressed()
    {
        foreach (var joker in _currentDisplay)
        {
            _shopMasterPool.Add(joker); //
        }
        
        GD.Print("Loja atualizada (reroll).");
        PopulateShop(); //
    }

    // --- 4. ADICIONE ESTE MÉTODO INTEIRO ---
    /// <summary>
    /// Desabilita os curingas da loja se o inventário estiver cheio.
    /// </summary>
    private void UpdateShopJokerState()
    {
        bool isFull = _playerInventory.Count >= MaxJokerSlots;

        // Itera sobre os curingas que ainda estão na loja
        foreach (var joker in _currentDisplay)
        {
            if (isFull && !joker.IsSelected)
            {
                // Desabilita (cinza)
                joker.MouseFilter = MouseFilterEnum.Ignore;
                joker.Modulate = new Color(0.5f, 0.5f, 0.5f); 
            }
            else
            {
                // Habilita (normal)
                joker.MouseFilter = MouseFilterEnum.Stop; 
                joker.Modulate = new Color(1f, 1f, 1f);
            }
        }
    }

    public List<JokerCard> GetUpdatedMasterPool()
    {
        foreach (var joker in _currentDisplay)
        {
            joker.OnCardClicked -= OnJokerClicked; //

            if (joker.GetParent() != null)
            {
                joker.GetParent().RemoveChild(joker); //
            }
            _shopMasterPool.Add(joker); //
        }
        
        _shopMasterPool = _shopMasterPool.OrderBy(x => GD.Randi()).ToList();
        return _shopMasterPool;
    }

    public List<JokerCard> GetUpdatedInventory()
    {
        return _playerInventory;
    }
}