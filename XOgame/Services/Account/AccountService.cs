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
            await _context.Users.AddAsync(new User {Nickname = input.Nickname});
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }
}