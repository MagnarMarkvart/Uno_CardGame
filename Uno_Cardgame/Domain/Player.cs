using Domain.Database;

namespace Domain;

public class Player
{
    public Guid PlayerId { get; set; } = Guid.NewGuid();
    
    public string? NickName { get; }
    public EPlayerType PlayerType { get; init; }
    public List<GameCard> PlayerHand { get; }
    public GameCard? HasDrawnACard { get; set; }
    public bool HasMadeAMove { get; set; }
    public bool HasCalledUno { get; set; }
    public int NeedsToDrawCards { get; set; } 
    public bool HasBeenIntroduced { get; set; }
    public bool NeedsToPickAColour { get; set; }
    
    
    public Player(string? nickName, List<GameCard> playerHand)
    {
        NickName = nickName;
        PlayerHand = playerHand;
    }
}