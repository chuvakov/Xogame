using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class Room : Entity
{
    public string Name { get; set; }
    //навигационное св-во (для джойна)
    public virtual ICollection<User> Users { get; set; }
    
    public virtual int? CurrentGameId { get; set; }
    [ForeignKey("CurrentGameId")]
    public virtual Game CurrentGame { get; set; }
    
    public virtual ICollection<Game> Games { get; set; }
}