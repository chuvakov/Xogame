using System.Net;
using Microsoft.AspNetCore.Mvc;
using XOgame.Common.Exceptions;
using XOgame.Services.Player;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PlayersController : ControllerBase
{
    private readonly IPlayerService _playerService;

    public PlayersController(IPlayerService playerService)
    {
        _playerService = playerService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> ChangeReady(string nickname)
    {
        try
        {
            var result = await _playerService.ChangeReady(nickname);
            return Ok(result);
        }
        catch (Exception e)
        {
            var message = "Не удалось сменить статус готовности";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetFigureType(string nickname)
    {
        return Ok(await _playerService.GetFigureType(nickname));
    }
}