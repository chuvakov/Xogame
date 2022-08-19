using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using XOgame.Common.Exceptions;
using XOgame.Services.Game;
using XOgame.Services.Game.Dto;

namespace XOgame.Controllers;

[Authorize]
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
        try
        {
            return Ok(await _service.DoStep(input));
        }
        catch (Exception e)
        {
            var message = "Не удалось сделать ход";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> Get(string roomName)
    {
        return Ok(await _service.Get(roomName));
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> StartGame(string roomName)
    {
        try
        {
            await _service.StartGame(roomName);
            return Ok();
        }
        catch (Exception e)
        {
            var message = "Не удалось начать игру";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }
}