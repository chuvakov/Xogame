using System.ComponentModel.DataAnnotations.Schema;
using XOgame.Core.Enums;

namespace XOgame.Core.Models;

public class User : Entity
{
    public string Nickname { get; set; }
    
    public int? CurrentRoomId { get; set; }
    //связь один ко многим в EFC (у 1го игрока 1 комната)
    [ForeignKey("CurrentRoomId")]
    public Room CurrentRoom { get; set; } 
    
    public bool IsReady { get; set; }
    public Role Role { get; set; }

    public SettingsSound SettingsSound { get; set; }
}