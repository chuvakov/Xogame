using Microsoft.EntityFrameworkCore;
using XOgame.Core;
using XOgame.Core.Models;
using XOgame.Extensions;
using XOgame.Services.Account.Dto;

namespace XOgame.Services.Account;

public class AccountService : IAccountService
{
    private readonly XOgameContext _context;
    private readonly ILogger<AccountService> _logger;

    public AccountService(XOgameContext context, ILogger<AccountService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> IsExist(AccountInput input)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == input.Nickname);
            return user != null;
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    public async Task Create(AccountInput input)
    {
        try
        {
            var user = new User
            {
                Nickname = input.Nickname,
                SettingsSound = new SettingsSound()
                {
                    IsEnabledDraw = true,
                    IsEnabledLose = true,
                    IsEnabledStep = true,
                    IsEnabledWin = true,
                    IsEnabledStartGame = true
                }
            };
            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }
}