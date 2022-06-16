namespace XOgame.Core.Models;

public class Room : Entity
{
    public string Name { get; set; }
    //навигационное св-во (для джойна)
    public virtual ICollection<User> Users { get; set; }
}