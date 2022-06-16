using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class User : Entity
{
    public string Nickname { get; set; }
    
    public int? CurrentRoomId { get; set; }
    [ForeignKey("CurrentRoomId")]
    public Room CurrentRoom { get; set; } 
}