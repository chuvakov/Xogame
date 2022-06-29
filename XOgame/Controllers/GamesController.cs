using Microsoft.AspNetCore.Mvc;
using XOgame.Services.Game;
using XOgame.Services.Game.Dto;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class GamesController : ControllerBase
{
    private readonly IGameService _service;

    public GamesController(IGameService service)
    {
        _service = service;
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> DoStep(DoStepInput input)
    {
        return Ok(await _service.DoStep(input));
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> Get(string roomName)
    {
        return Ok(await _service.Get(roomName));
    }
}