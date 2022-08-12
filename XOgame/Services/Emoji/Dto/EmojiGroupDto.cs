namespace XOgame.Services.Emoji.Dto;

public class EmojiGroupDto
{
    public string Name { get; set; }
    public IEnumerable<EmojiDto> Emojis { get; set; }
}