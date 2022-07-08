using System.Net;
using Microsoft.AspNetCore.Mvc;
using XOgame.Extensions;
using XOgame.Services;
using XOgame.Services.Account.Dto;
using XOgame.Services.Player;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;
    private readonly IPlayerService _playerService;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger, IPlayerService playerService)
    {
        _accountService = accountService;
        _logger = logger;
        _playerService = playerService;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(AccountInput input)
    {
        try
        {
            if (await _playerService.IsPlayerInRoom(input.Nickname))
            {
                return BadRequest("Ник занят");
            }
            var isAccountExist = await _accountService.IsExist(input);
            if (!isAccountExist) await _accountService.Create(input);

            return Ok();
        }
        catch (Exception e)
        {
            _logger.Error(e, "ошибка при авторизации");
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }
}