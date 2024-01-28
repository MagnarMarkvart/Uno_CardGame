namespace Domain;

public class GameState
{
    // GameState
    public Guid GameStateId { get; set; } = Guid.NewGuid();
    
    public List<Player> Players { get; set; } = new();
    public List<GameCard> DeckOfCards { get; set; } = new();
    public Stack<GameCard> CardsOnTable { get; set; } = new();
    public Player? GameWinner { get; private set; }
    public int ActivePlayerNo { get; set; }
    // GameState End

    public Player ShowNextPlayer()
    {
        return ActivePlayerNo < Players.Count - 1 ? Players[ActivePlayerNo + 1] : Players[0];
    }
    
    public Player ShowCurrentPlayer()
    {
        return Players[ActivePlayerNo];
    }

    public Player ShowLastPlayer()
    {
        return ActivePlayerNo == 0 ? Players.Last() : Players[ActivePlayerNo - 1];
    }

    public bool ContainsHumanPlayers()
    {
        return Players.Any(player => player.PlayerType == EPlayerType.Human);
    }

    public void NextPlayer()
    {
        var currentPlayer = ShowCurrentPlayer();
        currentPlayer.HasMadeAMove = false;
        currentPlayer.HasCalledUno = false;
        currentPlayer.HasDrawnACard = null;
        currentPlayer.HasBeenIntroduced = false;
        
        if (!currentPlayer.PlayerHand.Any())
        {
            GameWinner = currentPlayer;
            return;
        }
        
        if (ActivePlayerNo == Players.Count - 1)
        {
            ActivePlayerNo = 0;
        }
        else
        {
            ActivePlayerNo++;
        }

        if (ContainsHumanPlayers())
        {
            ShowCurrentPlayer().PlayerHand[0].IsSelected = true;
        }
    }

    public void ReverseOrder()
    {
        var activePlayer = ShowCurrentPlayer();
        Players.Reverse();
        ActivePlayerNo = Players.IndexOf(activePlayer);
    }
    
    
}