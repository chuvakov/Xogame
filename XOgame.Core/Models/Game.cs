using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class Game : Entity
{
    public int? RoomId { get; set; }
    // связь один к одному (у 1 игры 1 комната)
    [ForeignKey("RoomId")]
    public Room Room { get; set; }
    
    // связь один к одному (у 1й игры 1 победитель-юзер)
    public int? WinnerId { get; set; }
    [ForeignKey("WinnerId")]
    public User Winner { get; set; }
    
    public int? UserTurnId { get; set; }
    [ForeignKey("UserTurnId")]
    public User UserTurn { get; set; }
    
    public virtual ICollection<GameProgress> GameProgresses { get; set; }
    public virtual ICollection<UserGame> UserGames { get; set; }
}