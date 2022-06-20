using System.Net;
using Microsoft.AspNetCore.Mvc;
using XOgame.Extensions;
using XOgame.Services;
using XOgame.Services.Account.Dto;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;
    private readonly ILogger<AccountController> _logger;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger)
    {
        _accountService = accountService;
        _logger = logger;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(AccountInput input)
    {
        try
        {
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