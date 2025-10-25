using Godot;
using System;
using System.Collections.Generic;
using System.Linq;

public partial class ShopController : Control
{
    [Export] private NodePath JokerListContainerPath; // O 'JokerList' da sua imagem
    [Export] private NodePath BuyButtonPath;
    [Export] private NodePath RerollButtonPath;
    
    // Quantos curingas mostrar na loja
    private const int ShopDisplayCount = 3;

    private HBoxContainer _jokerListContainer;
    private Button _buyButton;
    private Button _rerollButton;

    // Listas internas da loja (cópias das listas do GameManager)
    private List<JokerCard> _shopMasterPool = new();
    private List<JokerCard> _playerInventory = new();

    // Curingas que estão sendo exibidos no momento
    private List<JokerCard> _currentDisplay = new();

    // Chamado pelo GameManager para iniciar a loja
    public void Initialize(List<JokerCard> masterPool, List<JokerCard> playerInventory)
    {
        // Cria cópias locais para não modificar as listas principais do GameManager
        // até que o jogador saia da loja (clicando em "Seguir")
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

        // --- INÍCIO DA CORREÇÃO ---
        // Estas duas linhas estavam faltando no seu arquivo.
        _buyButton.Pressed += OnBuyPressed;
        _rerollButton.Pressed += OnRerollPressed;
        // --- FIM DA CORREÇÃO ---

        await ToSignal(GetTree(), "process_frame");
        
        PopulateShop();
    }

    private void PopulateShop()
    {
        // 1. Limpa os curingas atuais (sem dar QueueFree)
        foreach (var joker in _currentDisplay)
        {
            joker.OnCardClicked -= OnJokerClicked; // Desconecta o clique
            if (joker.IsSelected) joker.ToggleSelection(); // Garante que não está selecionado
            _jokerListContainer.RemoveChild(joker);
        }
        _currentDisplay.Clear();

        // 2. Define o "baralho" da loja:
        // Pega todos os curingas do pool principal...
        // ...exceto os que o jogador já tem no inventário.
        var availablePool = _shopMasterPool
            .Where(joker => !_playerInventory.Any(owned => owned.Name == joker.Name))
            .ToList();

        // 3. Embaralha o baralho disponível
        availablePool = availablePool.OrderBy(x => GD.Randi()).ToList();

        // 4. Pega os 3 primeiros
        int count = Math.Min(availablePool.Count, ShopDisplayCount);
        _currentDisplay = availablePool.Take(count).ToList();

        // 5. Adiciona os novos curingas à cena e remove do pool
        foreach (var joker in _currentDisplay)
        {
            // Remove do pool principal da loja (para não ser sorteado de novo)
            _shopMasterPool.Remove(joker); 
            
            // Adiciona ao container HBox
            _jokerListContainer.AddChild(joker);
            
            // Conecta o evento de clique
            joker.OnCardClicked += OnJokerClicked;
        }
    }

    // Chamado quando o jogador clica em um curinga na loja
    private void OnJokerClicked(BaseCard clickedCard)
    {
        // Permite selecionar/desselecionar
        clickedCard.ToggleSelection();
    }

    // Chamado quando clica em "Comprar"
    private void OnBuyPressed()
    {
        // Pega os curingas selecionados
        var boughtJokers = _currentDisplay.Where(j => j.IsSelected).ToList();
        
        if (boughtJokers.Count == 0)
        {
            GD.Print("Nenhum curinga selecionado para compra.");
            return;
        }
        
        foreach (var joker in boughtJokers)
        {
            GD.Print($"Jogador comprou {joker.Name}.");
            
            // 1. Adiciona ao inventário do jogador
            _playerInventory.Add(joker);
            
            // 2. Remove da lista de exibição atual
            _currentDisplay.Remove(joker);
            
            // 3. Remove da cena (container HBox)
            _jokerListContainer.RemoveChild(joker);
            
            // 4. Limpa eventos e seleção
            joker.OnCardClicked -= OnJokerClicked;
            if (joker.IsSelected) joker.ToggleSelection(); // Desseleciona
        }
    }

    // Chamado quando clica em "Atualizar" (Reroll)
    private void OnRerollPressed()
    {
        // 1. Devolve os curingas em exibição (que não foram comprados)
        // de volta ao pool principal da loja.
        foreach (var joker in _currentDisplay)
        {
            _shopMasterPool.Add(joker);
        }
        
        GD.Print("Loja atualizada (reroll).");
        
        // 2. Repopula a loja (que vai limpar os antigos e puxar novos)
        PopulateShop();
    }

    // Métodos para o GameManager recuperar as listas atualizadas
    
    // Retorna o pool principal com os curingas descartados e não comprados
    public List<JokerCard> GetUpdatedMasterPool()
    {
        // Adiciona de volta os que estavam em exibição mas não foram comprados
        foreach (var joker in _currentDisplay)
        {
            // Desinscreve o evento de clique antes de devolver ao pool
            // Isso evita a "inscrição zumbi" na próxima rodada.
            joker.OnCardClicked -= OnJokerClicked;

            // Remove o curinga da hierarquia da loja ANTES que a loja
            // seja destruída, para que o curinga não seja 'disposed'.
            if (joker.GetParent() != null)
            {
                joker.GetParent().RemoveChild(joker);
            }

            // Adiciona de volta à lista mestre
            _shopMasterPool.Add(joker);
        }
        
        _shopMasterPool = _shopMasterPool.OrderBy(x => GD.Randi()).ToList();
        return _shopMasterPool;
    }

    // Retorna o inventário do jogador com os novos curingas comprados
    public List<JokerCard> GetUpdatedInventory()
    {
        return _playerInventory;
    }
}