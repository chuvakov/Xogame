using Microsoft.AspNetCore.Mvc;
using XOgame.Core;
using XOgame.Services;
using XOgame.Services.Account.Dto;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController : ControllerBase
{
    private readonly IAccountService _accountService;

    public AccountController(IAccountService accountService)
    {
        _accountService = accountService;
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> Login(AccountInput input)
    {
        bool isAccountExist = await _accountService.IsExist(input);
        if (!isAccountExist )
        {
            await _accountService.Create(input);
        }
        
        return Ok();
    }
}