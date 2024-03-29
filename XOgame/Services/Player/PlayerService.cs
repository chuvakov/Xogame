using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Core.Enums;
using XOgame.Extensions;
using XOgame.Services.Game;

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

    public async Task<char> GetFigureType(string nickname)
    {
        var user = await _context.Users
            .Include(u => u.CurrentRoom)
            .SingleOrDefaultAsync(u => u.Nickname == nickname);
        var userGame = await _context.UserGames.SingleOrDefaultAsync(ug =>
            ug.UserId == user.Id && ug.GameId == user.CurrentRoom.CurrentGameId);

        return userGame.FigureType == FigureType.Cross ? 'X' : 'O';
    }

    public async Task<bool> IsPlayerInRoom(string nickname)
    {
        var player = await _context.Users.SingleOrDefaultAsync(p => p.Nickname == nickname);

        if (player == null)
        {
            throw new UserFriendlyException($@"Пользователя с никком ""{nickname}"" нет", -100);
        }

        return player.CurrentRoomId.HasValue;
    }
}