using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XOgame.Core;
using XOgame.Core.Models;
using XOgame.Services.Emoji;
using XOgame.Services.Emoji.Dto;

namespace XOgame.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class EmojiController : ControllerBase
{
    private readonly IEmojiService _service;
    private readonly XOgameContext _context;

    public EmojiController(IEmojiService service, XOgameContext context)
    {
        _service = service;
        _context = context;
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAllGroups()
    {
        return Ok(await _service.GetAllGroups());
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> AddGroups(List<EmojiGroupDto> groups)
    {
        foreach (var g in groups)
        {
            var group = new EmojiGroup
            {
                Name = g.Name
            };
            _context.EmojiGroups.Add(group);
            await _context.SaveChangesAsync();

            foreach (var e in g.Emojis)
            {
                _context.Emojis.Add(new Emoji
                {
                    EmojiGroupId = group.Id,
                    Name = e.Name,
                    Text = e.Text
                });
            }

            await _context.SaveChangesAsync();
        }

        return Ok();
    }
}