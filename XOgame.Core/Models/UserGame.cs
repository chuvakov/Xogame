using System.ComponentModel.DataAnnotations.Schema;
using XOgame.Core.Enums;

namespace XOgame.Core.Models;

public class UserGame : Entity
{
    public int UserId { get; set; }
    [ForeignKey("UserId")]
    public User User { get; set; }
    
    public int GameId { get; set; }
    [ForeignKey("GameId")]
    public Game Game { get; set; }
    
    public FigureType FigureType { get; set; }
}