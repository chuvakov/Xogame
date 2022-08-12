using System.ComponentModel.DataAnnotations.Schema;

namespace XOgame.Core.Models;

public class Emoji : Entity
{
    public int EmojiGroupId { get; set; }
    [ForeignKey("EmojiGroupId")]
    public EmojiGroup EmojiGroup { get; set; }
    public string Name { get; set; }
    public string Text { get; set; }
}