namespace Domain.Database;

public class Game
{
    public Guid GameId { get; set; }
    
    public string GameState { get; set; } = default!;

    public DateTime CreatedAt { get; set; } = default!;
    public DateTime UpdatedAt { get; set; } = default!;
    
    public ICollection<Player>? Players { get; set; }
}