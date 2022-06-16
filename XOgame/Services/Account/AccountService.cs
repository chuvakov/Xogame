using Microsoft.EntityFrameworkCore;
using XOgame.Core;
using XOgame.Core.Models;
using XOgame.Services.Account.Dto;

namespace XOgame.Services.Account;

public class AccountService : IAccountService
{
    private readonly XOgameContext _context;

    public AccountService(XOgameContext context)
    {
        _context = context;
    }
    
    public async Task<bool> IsExist(AccountInput input)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == input.Nickname);
        return user != null;
    }

    public async Task Create(AccountInput input)
    {
        await _context.Users.AddAsync(new User(){Nickname = input.Nickname});
        await _context.SaveChangesAsync();
    }
}