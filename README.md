<p align="center">
  <img width="322" height="184" alt="logo05" src="https://github.com/user-attachments/assets/b9b004b6-57d3-4e1d-9fa7-fbb667b12844" />
</p>
1. Objetivo do Jogo
O objetivo do jogo é formar mãos com combinações de cartas que gerem a maior pontuação possível, manipular o deck e criar estratégias. Ao final de cada rodada, se o jogador atingir a pontuação necessária, ele vence.

2 Conceitos Básicos



2.1 Cartas



O jogo utiliza um baralho padrão de 52 cartas (sem curingas, salvo variação combinada).

Cada carta tem valor numérico e naipe.


Valores:

Números 2 a 10: valor nominal.
A (Ás) 11 pontos
Q (Dama), K (Rei) e J  (Valete): 10 pontos

2.2 Mão
Cada jogador recebe uma quantidade fixa de cartas (definida no início da partida, ex: 8 cartas).
A mão é composta pelas cartas que o jogador possui na sua vez.
O jogador deve analisar sua mão para formar combinações de valor.



2.3 Combinações
Sequência (Straight): cartas consecutivas do mesmo naipe. Ex: 5♠ 6♠ 7♠.
Trinca (Three of a Kind): três cartas de mesmo valor. Ex: 7♣ 7♦ 7♥.
Quadra (Four of a Kind): quatro cartas de mesmo valor.
Flush: cinco cartas do mesmo naipe, não necessariamente em sequência.



2.4 Blinds e Ante
Blinds e ante são como as “fases” do jogo, cada ante tem 3 blinds diferentes small, big e boss. Ao iniciar o jogo o jogador estará na ante 1 small blind, caso o jogador consiga atingir a pontuação necessária, ele avança para a ante 1 big blind, e caso consiga avançar novamente ele avança ao boss blind, caso derrote o boss blind o jogador avança para a ante 2, qual tem a mesma organização (small, big, boss), esse ciclo se repete até a ante 8, onde caso o jogador derrote o boss ele vence o jogo.
Os Boss blinds são como o nome diz “Chefões”, estes adicionam um “debuff” espécie de efeito negativo ao deck do jogador quando o enfrenta, por exemplo todas as cartas do naipe de copas são debuffadas, se o jogador joga uma mão que contenha uma carta de copas, está na terá efeito na pontuação.


3. Pontuação

Cada combinação possui um valor de pontuação específico:
<p align="center">
  <img width="495" height="429" alt="18711e15-3bd8-4984-a862-8d6b573f57ba" src="https://github.com/user-attachments/assets/640d4d83-858a-4a80-9ef7-c14cf7eea8dd" />
</p>



4. Descarte



4.1 Objetivo do Descarte
O objetivo do descarte é descartar cartas que no momento não favorecem a criação de combinações fortes, dando a chance de receber cartas  melhores após o descarte.




4.2 Regras do Descarte
O Jogador começa uma rodada com 3 descartes, os descartes podem ser feitos de 1 até 5 cartas por descarte, após descartar as cartas selecionadas essas não poderão ser mais utilizadas na rodada. O mesmo número de cartas descartadas serão compradas se disponíveis no deck. Por exemplo, se o jogador descarta 5 cartas mas restam apenas 3 cartas no deck, as 3 cartas serão compradas.



5. Loja
A Loja é o espaço do jogo onde o jogador pode gastar dinheiro ($) para adquirir Coringas. Ela não está sempre disponível, sendo acessível apenas após a vitória contra determinados desafios, como o Small Blind, o Big Blind ou o Boss Blind. Dessa forma, a Loja funciona como uma fase de compras entre batalhas, permitindo que o jogador fortaleça sua estratégia antes de prosseguir.


 
5.1 Reroll
O Reroll é uma mecânica que permite ao jogador pagar dinheiro para trocar as cartas disponíveis na loja por novas opções. Cada vez que o jogador dá um reroll, duas novas cartas aleatórias aparecem no lugar das anteriores. O preço desse recurso começa em $5 e aumenta progressivamente em $1 a cada uso, mas sempre volta para $5 quando uma nova loja é acessada. 



6. Curingas

<p align="center">
  <img width="57" height="79" alt="C_opcional1" src="https://github.com/user-attachments/assets/41cf023d-7d55-41d4-a02a-43c81d312773" />
</p>


Curingas são a “ferramenta” principal do jogo, eles são capazes de gerar pontuação, manipulação do deck e até gerar economia. 
Os curingas não são jogados junto às cartas do baralho, eles podem ser comprados na loja, o jogador tem um mão de coringas, a qual suporta até 5 curingas, o jogador pode vender um ou mais coringa a qualquer momento.
Curingas possuem diferentes classificações de raridade, quanto mais raro mais difícil aparecerem na loja, sendo Comum 70%, Incomum 25%, Raro 4,5% e Lendário 0,5%.

<p align="center">
  <strong>Alguns exemplos dos 4 pilares da Programação Orientada a Objetos (POO)</strong>
</p>

<p align="center">
  <strong>Encapsulamento 🔒 </strong>
</p>

Em CardData.cs, a classe CardData encapsula os atributos Suit, Rank, Name, TexturePath e ChipValue. 
Eles são definidos como propriedades com getters públicos e setters privados (ex.: public Suit Suit { get; private set; }), garantindo que só possam ser definidos no construtor e não alterados externamente. 
O método privado CalculateChipValue é usado internamente para computar ChipValue, escondendo a lógica de cálculo.

<p align="center">
  <strong>Herança 🌳 </strong>
</p>
Em Card.cs, a classe Card herda de BaseCard (public partial class Card : BaseCard). 
Ela herda propriedades como Name, TextureIcon e eventos como OnCardClicked, e adiciona funcionalidades específicas, como SetCard(CardData data, Texture2D texture) e GetChipValue(). 
Isso permite reutilizar o comportamento básico de cartas (como drag-and-drop) definido em BaseCard.

<p align="center">
  <strong>Polimorfismo 🔄 </strong>
</p>
Em BaseCard.cs, métodos como Initialize(string name, Texture2D texture) são virtuais (public virtual void Initialize...), permitindo que classes derivadas como Card e JokerCard os usem diretamente ou os estendam.
Eventos como OnCardClicked e OnDragging são delegados que podem ser invocados polimorficamente em qualquer instância de BaseCard ou derivadas, independentemente do tipo exato (ex.: em UIController.cs, OnCardClicked += OnCardClicked; é usado para cartas normais e curingas).


<p align="center">
  <strong>Abstração 📝 </strong>
</p>


BaseCard.cs é uma classe abstrata (public abstract partial class BaseCard : TextureRect), definindo comportamentos comuns como Initialize (marcado como virtual para abstração) e eventos para cliques/drags. 
Ela abstrai o conceito de "carta" genérica, sem detalhes específicos de cartas normais ou curingas, permitindo que Card e JokerCard implementem concretamente.
