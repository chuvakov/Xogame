using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Extensions;

namespace XOgame.Services.Player;

public class PlayerService : IPlayerService
{
    private readonly XOgameContext _context;
    private readonly ILogger<PlayerService> _logger;

    public PlayerService(XOgameContext context, ILogger<PlayerService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<bool> ChangeReady(string nickname)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

            if (user == null) throw new UserFriendlyException(@$"Пользователь с ником ""{nickname}"" не найден", -100);
            user.IsReady = !user.IsReady;
            await _context.SaveChangesAsync();
            return user.IsReady;
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }
}