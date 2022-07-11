using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class Room : Entity
{
    public string Name { get; set; }
    
    //навигационное св-во (для джойна) связь один ко многим в EFC (у 1й комнаты много игроков)
    public virtual ICollection<User> Users { get; set; }
    
    public virtual int? CurrentGameId { get; set; }
    // связь один к одному (у 1 комнаты 1 игра)
    [ForeignKey("CurrentGameId")]
    public virtual Game CurrentGame { get; set; }
    
    public virtual ICollection<Game> Games { get; set; }
}