using System.ComponentModel.DataAnnotations;

namespace Domain.Database;

public class Player
{
    public Guid PlayerId { get; set; }
    
    [MinLength(2, ErrorMessage = "Nickname too short!")]
    [MaxLength(32, ErrorMessage = "Nickname too long!")]
    public string NickName { get; set; } = default!;
    public EPlayerType PlayerType { get; set; }

    public Guid GameId { get; set; }
    public Game? Game { get; set; }
}