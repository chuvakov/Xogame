using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Core.Enums;
using XOgame.Core.Models;
using XOgame.Extensions;
using XOgame.Services.Game.Dto;
using XOgame.Services.Player.Dto;
using XOgame.SignalR;

namespace XOgame.Services.Game;

public class GameService : IGameService
{
    private readonly XOgameContext _context;
    private readonly IHubContext<GameHub> _gameHub;
    private readonly ILogger<GameService> _logger;

    private readonly int[][] _winnerPositions =
    {
        new[] {1, 2, 3},
        new[] {4, 5, 6},
        new[] {7, 8, 9},
        new[] {1, 4, 7},
        new[] {2, 5, 8},
        new[] {3, 6, 9},
        new[] {1, 5, 9},
        new[] {3, 5, 7}
    };

        public GameService(XOgameContext context, IHubContext<GameHub> gameHub, ILogger<GameService> logger)
        {
            _context = context;
            _gameHub = gameHub;
            _logger = logger;
        }

    public async Task<DoStepResultDto> DoStep(DoStepInput input)
    {
        var user = await _context.Users
            .Include(u => u.CurrentRoom)
            .FirstOrDefaultAsync(u => u.Nickname == input.Nickname);

        if (user == null)
        {
            throw new UserFriendlyException(@$"Пользователь с ником ""{input.Nickname}"" не найден");
        }

        if (user.CurrentRoom == null || !user.CurrentRoom.CurrentGameId.HasValue)
        {
            throw new UserFriendlyException(@$"Пользователь с ником ""{input.Nickname}"" не играет");
        }
        
        var gameId = user.CurrentRoom.CurrentGameId.Value;
        var game = await _context.Games
            .Include(g => g.UserGames)
            .SingleAsync(g => g.Id == gameId);

        if (game.WinnerId.HasValue)
        {
            throw new UserFriendlyException("Данная игра является завершенной!");
        }

        if (game.UserTurn != user)
        {
            throw new UserFriendlyException("Сейчас не ваш ход!");
        }

        var isExistStep = _context.GameProgresses.Any(gp => gp.GameId == gameId && gp.CellNumber == input.CellNumber);
        if (isExistStep)
        {
            throw new UserFriendlyException("Данная ячейка занята!");
        }

        await _context.GameProgresses.AddAsync(new GameProgress()
        {
            UserId = user.Id,
            CellNumber = input.CellNumber,
            GameId = gameId
        });

        await _context.SaveChangesAsync();

        var gameProgresses = await _context.GameProgresses
            .Where(gp => gp.GameId == gameId)
            .ToListAsync();

        var result = new DoStepResultDto();
        
        if (gameProgresses.Count == 9)
        {
            result.IsFinish = true;
        }

        var numberSteps = gameProgresses
            .Where(gp => gp.UserId == user.Id)
            .Select(gp => gp.CellNumber)
            .ToArray();

        foreach (var winnerPosition in _winnerPositions)
        {
            var resultPosition = numberSteps
                .Intersect(winnerPosition)
                .OrderBy(n => n)
                .ToArray();

            if (Enumerable.SequenceEqual(resultPosition, winnerPosition))
            {
                result.IsWinner = true;
                result.IsFinish = true;

                foreach (var userGame in game.UserGames)
                {
                    var player = await _context.Users.SingleAsync(u => u.Id == userGame.UserId);
                    player.IsReady = false;
                    await _context.SaveChangesAsync();
                    
                    if (user.Id == player.Id)
                    {
                        await _gameHub.Clients.All.SendAsync("GameFinished-" + player.Nickname, true);
                    }
                    else
                    {
                        await _gameHub.Clients.All.SendAsync("GameFinished-" + player.Nickname, false);
                    }
                }

                user.CurrentRoom.CurrentGameId = null;
                await _context.SaveChangesAsync();
                
                return result;
            }
        }

        game.UserTurnId = game.UserGames.Single(ug => ug.UserId != user.Id).UserId;
        await _context.SaveChangesAsync();

        return result;
    }

    public async Task<GameDto> Get(string roomName)
    {
        var room = await _context.Rooms
            .Include(r => r.Users)
            .SingleOrDefaultAsync(r => r.Name == roomName);

        if (room == null)
        {
            throw new UserFriendlyException(@$"Комната с названием ""{roomName}"" не найдена!");
        }

        var game = await _context.Games
            .Include(g => g.GameProgresses)
            .Include(g => g.UserGames)
            .SingleOrDefaultAsync(g => g.Id == room.CurrentGameId);

        var result = new GameDto()
        {
            Players = room.Users.Select(u => new PlayerShortDto()
            {
                Nickname = u.Nickname
            }),
        };

        if (game.UserTurnId.HasValue)
        {
            result.PlayerTurnNickname = room.Users.Single(u => u.Id == game.UserTurnId.Value).Nickname;
        }

        var steps = new List<StepDto>();
        foreach (var gameProgress in game.GameProgresses)
        {
            var step = new StepDto()
            {
                CellNumber = gameProgress.CellNumber
            };

            var figureType = game.UserGames.Single(ug => ug.UserId == gameProgress.UserId).FigureType;
            step.FigureType = figureType == FigureType.Cross ? 'X' : 'O';
            
            steps.Add(step);
        }

        result.Steps = steps;

        return result;
    }
    
    //Вспомогательный метод создающий запись в таблице Игра
    public async Task StartGame(int playerFirstId, int playerSecondId, int roomId)
    {
        try
        {
            var game = new Core.Models.Game
            {
                RoomId = roomId,
                UserTurnId = playerFirstId
            };

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            await _context.UserGames.AddAsync(new UserGame
            {
                UserId = playerFirstId,
                GameId = game.Id,
                FigureType = FigureType.Cross
            });

            await _context.UserGames.AddAsync(new UserGame
            {
                UserId = playerSecondId,
                GameId = game.Id,
                FigureType = FigureType.Nought
            });

            var room = _context.Rooms.FirstOrDefault(r => r.Id == roomId);

            if (room != null)
            {
                room.CurrentGameId = game.Id;
            }

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw new UserFriendlyException("Не удалось запустить игру", -100);
        }
    }
}