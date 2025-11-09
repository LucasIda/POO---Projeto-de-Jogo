<h1 align="center">🎴 Royal Cards</h1>

<h2>1. Objetivo do Jogo</h2>
<p>
O objetivo do jogo é formar mãos com combinações de cartas que gerem a maior pontuação possível, manipular o deck e criar estratégias. 
Ao final de cada rodada, se o jogador atingir a pontuação necessária, ele vence, passando para a próxima fase.
O objetivo é alcançar a maior pontuação possível, utilizando os curingas à disposição.
</p>

<hr>

<h2>2. Conceitos Básicos</h2>

<p align="center">
  <img width="1919" height="1079" alt="Image" src="https://github.com/user-attachments/assets/429fa7b9-7579-489a-a34c-ff7b58c2f16b" />
</p>

<h3>2.1 Cartas</h3>
<p>
O jogo utiliza um baralho padrão de 52 cartas (sem curingas, salvo variação combinada). 
Cada carta tem valor numérico e naipe.
</p>
<p>
<b>Valores:</b><br>
Números 2 a 10: valor nominal.<br>
A (Ás): 11 pontos.<br>
Q (Dama), K (Rei) e J (Valete): 10 pontos.
</p>

<h3>2.2 Mão</h3>
<p>
Cada jogador recebe uma quantidade fixa de cartas (8 cartas).<br>
A mão é composta pelas cartas que o jogador possui na sua vez.<br>
O jogador deve analisar sua mão para formar combinações de valor.
</p>

<h3>2.3 Combinações</h3>
<ul>
  <li><b>Sequência (Straight):</b> cartas consecutivas do mesmo naipe. Ex: 5♠ 6♠ 7♠</li>
  <li><b>Trinca (Three of a Kind):</b> três cartas de mesmo valor. Ex: 7♣ 7♦ 7♥</li>
  <li><b>Quadra (Four of a Kind):</b> quatro cartas de mesmo valor.</li>
  <li><b>Flush:</b> cinco cartas do mesmo naipe, não necessariamente em sequência.</li>
</ul>

<h3>2.4 Blinds e Ante</h3>
<p>
Blinds e ante são como as “fases” do jogo, cada ante tem 3 blinds diferentes: small, big e boss.
</p>
<p>
Ao iniciar o jogo o jogador estará na ante 1 small blind. Caso o jogador consiga atingir a pontuação necessária, ele avança para a ante 1 big blind; 
se conseguir novamente, avança ao boss blind. 
Ao derrotar o boss blind, o jogador avança para a ante 2 — o ciclo se repete até a ante 12.
</p>

<hr>

<h2>3. Pontuação</h2>
<p>Cada combinação possui um valor de pontuação específico:</p>

<p align="center">
  <img width="1024" height="976" alt="Image" src="https://github.com/user-attachments/assets/8e4cce9d-5436-48c9-916f-f643c4a37293" />
</p>

<hr>

<h2>4. Descarte</h2>

<h3>4.1 Objetivo do Descarte</h3>
<p>
O objetivo do descarte é eliminar cartas que, no momento, não favorecem a criação de combinações fortes, 
dando a chance de receber cartas melhores após o descarte.
</p>

<h3>4.2 Regras do Descarte</h3>
<p>
O jogador começa uma rodada com 3 descartes. Cada descarte pode ser de 1 até 5 cartas.<br>
Após descartar, essas cartas não poderão mais ser utilizadas na rodada.<br>
O mesmo número de cartas descartadas será comprado do deck (se disponível).<br>
Exemplo: se o jogador descarta 5 cartas, mas restam apenas 3 no deck, ele comprará apenas 3.
</p>

<hr>

<h2>5. Loja</h2>

<p align="center">
  <img width="1433" height="529" alt="Image" src="https://github.com/user-attachments/assets/d0a069e4-aa5f-4d57-87f4-83c532b8e945" />
</p>

<p>
A Loja é o espaço onde o jogador pode gastar dinheiro (<b>$</b>) para adquirir Curingas.<br>
Ela aparece apenas após vitórias em determinadas fases, como Small Blind, Big Blind ou Boss Blind.<br>
Funciona como uma fase de compras entre batalhas, permitindo fortalecer a estratégia antes de prosseguir.
</p>

<h3>5.1 Reroll</h3>
<p>
O <b>Reroll</b> permite ao jogador pagar dinheiro para trocar as cartas disponíveis na loja por novas opções.<br>
Cada reroll exibe três novas cartas aleatórias no lugar das anteriores.<br>
O custo começa em <b>$2</b> e aumenta em <b>$1</b> a cada uso, voltando para $2 ao abrir uma nova loja.
</p>

<hr>

<h2>6. Curingas</h2>

<p align="center">
  <img width="142" height="190" alt="Image" src="https://github.com/user-attachments/assets/cfe936f6-3b61-40a3-baa4-3742c3c51e64" />
</p>

<p>
Curingas são a principal ferramenta do jogo — capazes de gerar pontuação, manipular o deck e criar economia.<br>
Eles não são jogados junto às cartas do baralho comum.<br>
O jogador pode possuir até 5 curingas na mão e vender qualquer um a qualquer momento.
</p>
<p>
Curingas têm diferentes raridades, que afetam no seu custo para compra<br>
</p>

<hr>

<h2>7. Os 4 Pilares da POO no Projeto</h2>

<p>Os quatro pilares da Programação Orientada a Objetos (POO) foram aplicados da seguinte forma:</p>

<hr>

<h3>7.1 Encapsulamento</h3>
<p>
Aplicado para proteger dados críticos como o dinheiro do jogador (<code>PlayerCoins</code>), controlado pelo <code>GameManager.cs</code>.
Apenas métodos públicos (<code>AddCoins</code>, <code>SpendCoins</code>) podem alterar esse valor.
</p>

<pre><code>// O 'private set' garante que só o GameManager pode definir o valor
public int PlayerCoins { get; private set; } = 4;

public void AddCoins(int amount)
{
    if (amount &gt; 0)
        PlayerCoins += amount;
}

public void SpendCoins(int amount)
{
    if (amount &gt; PlayerCoins) return;
    PlayerCoins -= amount;
}
</code></pre>

<p><b>Motivo:</b> protege a variável de alterações indevidas e centraliza a lógica de validação.</p>

<hr>

<h3>7.2 Herança</h3>
<p>
Utilizada para reutilizar código entre <code>Card</code> e <code>JokerCard</code>, ambas derivadas da classe base <code>BaseCard.cs</code>.
</p>

<pre><code>public abstract partial class BaseCard : TextureRect
{
    public string Name { get; protected set; }
    public bool IsSelected { get; private set; }
}

public partial class Card : BaseCard
{
    public CardData Data { get; private set; }
}

public partial class JokerCard : BaseCard
{
    private List&lt;IJokerEffect&gt; _effects = new();
    public int Cost { get; private set; }
}
</code></pre>

<p><b>Motivo:</b> evita duplicação de código e mantém a lógica comum em um único local.</p>

<hr>

<h3>7.3 Abstração</h3>
<p>
Utilizamos a interface <code>IJokerEffect</code> para padronizar o comportamento dos efeitos de curingas, 
sem expor detalhes de implementação.
</p>

<pre><code>public interface IJokerEffect
{
    string Description { get; }
    void Apply(HandValue.HandResult result);
}
</code></pre>

<p><b>Implementações concretas:</b></p>

<pre><code>public class EffectAddChips : IJokerEffect
{
    private int _chips;
    public void Apply(HandValue.HandResult result)
    {
        result.ChipsBase += _chips;
    }
}
</code></pre>

<pre><code>public class EffectMultiplyMultiplier : IJokerEffect
{
    private float _factor;
    public void Apply(HandValue.HandResult result)
    {
        result.MultBase = (int)(result.MultBase * _factor);
    }
}
</code></pre>

<p><b>Motivo:</b> o <code>JokerCard</code> apenas chama <code>effect.Apply(result)</code> — sem precisar saber o tipo específico do efeito.</p>

<hr>

<h3>7.4 Polimorfismo</h3>
<p>
Usado para que <code>Card</code> e <code>JokerCard</code> tenham comportamentos diferentes ao sobrescrever métodos herdados da <code>BaseCard</code>.
</p>

<pre><code>protected virtual void HideTooltip() { }

protected override void HideTooltip()
{
    base.HideTooltip();
    if (tooltip != null)
    {
        tooltip.QueueFree();
        tooltip = null;
    }
}
</code></pre>

<p><b>Motivo:</b> permite comportamentos distintos para cada tipo de carta (ex: tooltips diferentes), usando a mesma chamada.</p>

<hr>

<p align="center">Desenvolvido pela Dots.Eng</p>
