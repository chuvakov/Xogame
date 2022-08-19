using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using XOgame.Authentication;
using XOgame.Common.Exceptions;
using XOgame.Core;
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
    private readonly XOgameContext _context;
    private readonly IConfiguration _configuration;

    public AccountController(IAccountService accountService, ILogger<AccountController> logger, IPlayerService playerService, XOgameContext context, IConfiguration configuration)
    {
        _accountService = accountService;
        _logger = logger;
        _playerService = playerService;
        _context = context;
        _configuration = configuration;
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Login(AccountInput input)
    {
        try
        {
            var token = await GetToken(input.Nickname, input.Password);
            return Ok(token);
        }
        catch (Exception e)
        {
            _logger.Error(e, "ошибка при авторизации");
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Register(AccountInput input)
    {
        try
        {
            var isAccountExist = await _accountService.IsExist(input);
            if (isAccountExist)
            {
                return BadRequest("Ник занят");
            }

            await _accountService.Create(input);
            return Ok();
        }
        catch (Exception e)
        {
            _logger.Error(e, "ошибка при регистрации");
            return StatusCode((int) HttpStatusCode.InternalServerError);
        }
    }

    private async Task<string> GetToken(string login, string password)
    {
        var identity = await GetIdentity(login, password);

        if (identity == null)
        {
            throw new UserFriendlyException("Не правильно введен логин или пароль", -100);
        }

        var authSettings = _configuration.GetSection("AuthSettings")
            .Get<AuthSettings>(opt => opt.BindNonPublicProperties = true);
            
        var nowTime = DateTime.UtcNow;
        var jwt = new JwtSecurityToken(
            issuer: authSettings.Issuer,
            audience: authSettings.Audience,
            notBefore: nowTime,
            expires: nowTime.Add(TimeSpan.FromMinutes(authSettings.LifeTime)),
            claims: identity.Claims,
            signingCredentials: new SigningCredentials(authSettings.SymmetricSecurityKey, SecurityAlgorithms.HmacSha256)
        );

        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
    
    private async Task<ClaimsIdentity> GetIdentity(string login, string password)
    {
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Nickname == login && u.Password == password);

        if (user == null)
        {
            return null;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimsIdentity.DefaultNameClaimType, user.Nickname),
            new Claim(ClaimsIdentity.DefaultRoleClaimType, user.Role.ToString()),
        };

        return new ClaimsIdentity(
            claims,
            "Token",
            ClaimsIdentity.DefaultNameClaimType,
            ClaimsIdentity.DefaultRoleClaimType
            );
    }
}