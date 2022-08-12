using XOgame.Core.Models;
using XOgame.Services.Emoji.Dto;

namespace XOgame.Services.Emoji;

public interface IEmojiService
{
    Task<IEnumerable<EmojiGroupDto>> GetAllGroups();
}