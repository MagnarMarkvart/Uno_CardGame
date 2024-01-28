using DAL;
using Domain;

namespace UnoEngine;

public class GameEngine
{
    // GameEngine Props
    private IGameRepository  GameRepository { get; }
    public GameState State { get; }
    private Random Rnd { get; } = new();
    public static bool ClassicRules => true;

    private const int InitialHandSize = 7;
    // GameEngine Props End

    // GameEngine Constructor
    public GameEngine(IGameRepository gameRepository, GameState state)
    {
        GameRepository = gameRepository;
        State = state;
    }
    
    public void Run()
    {
        if (!State.CardsOnTable.Any())
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        InitializeFullDeck();
        State.ActivePlayerNo = Rnd.Next(State.Players.Count);
        ShuffleDeck();
        Dealer();
        FirstDiscardEffect();
        if (State.ContainsHumanPlayers())
        {
            State.ShowCurrentPlayer().PlayerHand[0].IsSelected = true;
        }
    }

    private void InitializeFullDeck()
    {
        for (var cardColour = 0; cardColour < (int) ECardColour.Special; cardColour++)
        {
            for (var cardValue = 0; cardValue <= (int) ECardValue.DrawTwo; cardValue++)
            {
                InitializeCards(cardValue == 0 ? 1 : 2, cardColour, cardValue);
                
            }
        }
        
        for (var cardValue = (int) ECardValue.Wild; cardValue <= (int) ECardValue.WildDrawFour; cardValue++)
        {
            InitializeCards(4, 4, cardValue);
        }
    }
    
    private void InitializeCards(int count, int cardColour, int cardValue)
    {
        for (var i = 0; i < count; i++)
        {
            var card = new GameCard()
            {
                CardColour = (ECardColour)cardColour,
                CardValue = (ECardValue)cardValue
            };

            card.DrawCard = card.CardValue switch
            {
                ECardValue.DrawTwo      => 2,
                ECardValue.WildDrawFour => 4,
                _                       => 0
            };

            State.DeckOfCards.Add(card);
        }
    }

    private void ShuffleDeck()
    {
        var shuffledDeck = new List<GameCard>();

        while (State.DeckOfCards.Count > 0)
        {
            var randomPosInDeck = Rnd.Next(State.DeckOfCards.Count);
            shuffledDeck.Add(State.DeckOfCards[randomPosInDeck]);
            State.DeckOfCards.RemoveAt(randomPosInDeck);
            
        }

        State.DeckOfCards = shuffledDeck;
    }
    
    // Deal 7 cards to every player and set an initial card on the table.
    private void Dealer()
    {
        // Deal the cards to the players.
        foreach (var player in State.Players)
        {
            for (var handPointer = 0; handPointer < InitialHandSize; handPointer++)
            {
                player.PlayerHand.Add(TakeTopCard());
            }
        }
        // Set the initial card on the table.
        State.CardsOnTable.Push(TakeTopCard());
    }

    /* Takes the topmost card from the deck. 
       Removes from DeckOfCards
       Returns the GameCard */
    private GameCard TakeTopCard()
    {
        if (!State.DeckOfCards.Any())
        {
            RedoDeck();
        }
        var topCard = State.DeckOfCards.Last();
        State.DeckOfCards.RemoveAt(State.DeckOfCards.Count - 1);
        return topCard;
    }

    private void RedoDeck()
    {
        var topCard = State.CardsOnTable.Peek();
        InitializeFullDeck();
        State.DeckOfCards.Remove(topCard);
        ShuffleDeck();
    }

    public void PlayTheCard(Player activePlayer, GameCard card)
    {
        var nextPlayer = State.ShowNextPlayer();
        nextPlayer.NeedsToDrawCards = card.DrawCard;
        activePlayer.PlayerHand.Remove(card);

        ActionCard(card);
        
        if (card.CardColour == ECardColour.Special)
        {
            activePlayer.NeedsToPickAColour = true;
        }
        
        State.CardsOnTable.Push(card);
        activePlayer.HasMadeAMove = true;
    }

    public void DrawCardsFromDeck(Player player, int count)
    {
        if (count == 0) return;
        player.NeedsToDrawCards = 0;
        var cards = 0;
        var card = new GameCard();
        
        while (cards < count)
        {
            card = TakeTopCard();
            player.PlayerHand.Add(card);
            cards++;
        }

        player.HasDrawnACard = card;
    }

    public void MakeAiMove(Player player)
    {
        if (player.NeedsToDrawCards > 0 ||
            player.HasMadeAMove)
        {
            DrawCardsFromDeck(player, player.NeedsToDrawCards);
            State.NextPlayer();
            return;
        }
        
        var playerCards = new List<GameCard>(player.PlayerHand);

        foreach (var card in playerCards.Where
                     (card => MoveValidator.IsLegalMove(State, card)))
        {
            AiPlayingACard(player, card);

            if (!ClassicRules) continue;
            
            State.NextPlayer();
            return;
        }

        DrawCardsFromDeck(player, 1);
        AiPlayingACard(player, player.HasDrawnACard!);
        State.NextPlayer();
    }

    private void ActionCard(GameCard card)
    {
        var cardValue = card.CardValue;
        while (true)
        {
            switch (card.CardValue)
            {
                case ECardValue.Reverse:
                    if (State.Players.Count == 2)
                    {
                        card.CardValue = ECardValue.Skip;
                        continue;
                    }

                    State.ReverseOrder();
                    break;
                case ECardValue.Skip:
                    State.ShowNextPlayer().HasMadeAMove = true;
                    break;
                default:
                    return;
            }
            card.CardValue = cardValue;
            break;
        }
    }

    private void FirstDiscardEffect()
    {
        while (true)
        {
            var cardOnTable = State.CardsOnTable.Peek();
            
            if (cardOnTable.CardValue == ECardValue.WildDrawFour)
            {
                State.DeckOfCards.Add(State.CardsOnTable.Pop());
                ShuffleDeck();
                State.CardsOnTable.Push(TakeTopCard());
                continue;
            }
            
            if (cardOnTable.DrawCard != 0)
            {
                State.ShowCurrentPlayer().NeedsToDrawCards = cardOnTable.DrawCard;
                break;
            }

            if (cardOnTable.CardValue == ECardValue.Reverse)
            {
                State.ReverseOrder();
                State.NextPlayer();
                break;
            }

            if (cardOnTable.CardValue == ECardValue.Skip)
            {
                State.NextPlayer();
                break;
            }

            if (cardOnTable.CardValue == ECardValue.Wild)
            {
                if (State.ShowCurrentPlayer().PlayerType == EPlayerType.Human)
                {
                    State.ShowCurrentPlayer().NeedsToPickAColour = true;
                }
                else
                {
                    var rnd = Rnd.Next(4);
                    var card = State.CardsOnTable.Pop();
                    card.CardColour = (ECardColour) rnd;
                    State.CardsOnTable.Push(card);
                }
            }

            break;
        }
    }

    public bool ChallengeWildDrawFour()
    {
        var challengedCard = State.CardsOnTable.Pop();
        var lastCardColour = State.CardsOnTable.Peek().CardColour;
        State.CardsOnTable.Push(challengedCard);
        var challenger = State.ShowCurrentPlayer();
        var challenged = State.ShowLastPlayer();
        
        if (challenged.PlayerHand.Any(card => card.CardColour == lastCardColour))
        {
            DrawCardsFromDeck(challenged, 4);
            State.ShowCurrentPlayer().NeedsToDrawCards = 0;
            return true;
        }

        DrawCardsFromDeck(challenger, 6);
        State.ShowCurrentPlayer().HasMadeAMove = true;
        return false;
    }

    private void AiPlayingACard(Player player, GameCard card)
    {
        if (!MoveValidator.IsLegalMove(State, card)) return;
        
        if (card.CardColour == ECardColour.Special)
        {
            var rnd = Rnd.Next(4);
            card.CardColour = (ECardColour) rnd;
        }
        PlayTheCard(player, card);
    }
    
    public void SaveGame()
    {
        GameRepository.Save(State.GameStateId, State);
    }
}