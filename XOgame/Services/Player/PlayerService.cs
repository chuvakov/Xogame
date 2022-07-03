using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Extensions;
using XOgame.Services.Game;

namespace XOgame.Services.Player;

public class PlayerService : IPlayerService
{
    private readonly XOgameContext _context;
    private readonly ILogger<PlayerService> _logger;
    private readonly IGameService _gameService;

    public PlayerService(XOgameContext context, ILogger<PlayerService> logger, IGameService gameService)
    {
        _context = context;
        _logger = logger;
        _gameService = gameService;
    }

    public async Task<bool> ChangeReady(string nickname)
    {
        try
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Nickname == nickname);

            if (user == null) throw new UserFriendlyException(@$"Пользователь с ником ""{nickname}"" не найден", -100);
            user.IsReady = !user.IsReady;

            var roomId = user.CurrentRoomId;
            var opponent = await _context.Users.SingleOrDefaultAsync(u => u.CurrentRoomId == roomId && u.Id != user.Id);

            if (opponent != null && opponent.IsReady && user.IsReady)
            {
                await _gameService.StartGame(opponent.Id, user.Id, roomId.Value);
            }

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