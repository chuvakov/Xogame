using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class GameProgress : Entity
{
    public int GameId { get; set; }
    [ForeignKey("GameId")]
    public Game Game { get; set; }
    
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }

    public int CellNumber { get; set; }
}