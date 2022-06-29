using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class Game : Entity
{
    public int? RoomId { get; set; }
    [ForeignKey("RoomId")]
    public Room Room { get; set; }
    
    public int? WinnerId { get; set; }
    [ForeignKey("WinnerId")]
    public User Winner { get; set; }
    
    public int? UserTurnId { get; set; }
    [ForeignKey("UserTurnId")]
    public User UserTurn { get; set; }
    
    public virtual ICollection<GameProgress> GameProgresses { get; set; }
    public virtual ICollection<UserGame> UserGames { get; set; }
}