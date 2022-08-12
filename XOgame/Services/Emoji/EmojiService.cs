using Microsoft.EntityFrameworkCore;
using XOgame.Core;
using XOgame.Core.Models;
using XOgame.Services.Emoji.Dto;

namespace XOgame.Services.Emoji;

public class EmojiService : IEmojiService
{
    private readonly XOgameContext _context;

    public EmojiService(XOgameContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<EmojiGroupDto>> GetAllGroups()
    {
        var emojies = await _context
            .Emojis
            .Include(e => e.EmojiGroup)
            .ToListAsync();

        var groups = emojies.GroupBy(e => e.EmojiGroup.Name);
        var result = new List<EmojiGroupDto>();
        foreach (var group in groups)
        {
            result.Add(new EmojiGroupDto()
            {
                Name = group.Key,
                Emojis = group.Select(e => new EmojiDto
                {
                    Name = e.Name,
                    Text = e.Text
                })
            });
        }

        return result;
    }
}