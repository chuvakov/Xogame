using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class Game : Entity
{
    public int RoomId { get; set; }
    [ForeignKey("RoomId")]
    public Room Room { get; set; }
    
    public int? WinnerId { get; set; }
    [ForeignKey("WinnerId")]
    public User Winner { get; set; }
}