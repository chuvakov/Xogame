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

    private readonly WinnerPositionDto[] _winnerPositions =
    {
        new WinnerPositionDto()
        {
            Cells = new[] {1, 2, 3},
            Type = WinnerPositionType.Horizontal
        },
        new WinnerPositionDto()
        {
            Cells = new[] {4, 5, 6},
            Type = WinnerPositionType.Horizontal
        },
        new WinnerPositionDto()
        {
            Cells = new[] {7, 8, 9},
            Type = WinnerPositionType.Horizontal
        },
        new WinnerPositionDto()
        {
            Cells = new[] {1, 4, 7},
            Type = WinnerPositionType.Vertical
        },
        new WinnerPositionDto()
        {
            Cells = new[] {2, 5, 8},
            Type = WinnerPositionType.Vertical
        },
        new WinnerPositionDto()
        {
            Cells = new[] {3, 6, 9},
            Type = WinnerPositionType.Vertical
        },
        new WinnerPositionDto()
        {
            Cells = new[] {1, 5, 9},
            Type = WinnerPositionType.LeftSlash
        },
        new WinnerPositionDto()
        {
            Cells = new[] {3, 5, 7},
            Type = WinnerPositionType.RightSlash
        }
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
                .Intersect(winnerPosition.Cells)
                .OrderBy(n => n)
                .ToArray();

            if (Enumerable.SequenceEqual(resultPosition, winnerPosition.Cells))
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
                        await _gameHub.Clients.All.SendAsync("GameFinished-" + player.Nickname, new
                        {
                            result = GameResult.Win,
                            winnerPosition = winnerPosition
                        });
                    }
                    else
                    {
                        await _gameHub.Clients.All.SendAsync("GameFinished-" + player.Nickname, new
                        {
                            result = GameResult.Lose,
                            cell = input.CellNumber,
                            figureType = userGame.FigureType == FigureType.Cross ? 'O' : 'X',
                            winnerPosition = winnerPosition
                        });
                    }
                }

                user.CurrentRoom.CurrentGameId = null;
                await _context.SaveChangesAsync();
                
                return result;
            }
        }

        if (result.IsFinish)
        {
            foreach (var userGame in game.UserGames)
            {
                var player = await _context.Users.SingleAsync(u => u.Id == userGame.UserId);
                player.IsReady = false;
                await _context.SaveChangesAsync();
                    
                if (user.Id == player.Id)
                {
                    await _gameHub.Clients.All.SendAsync("GameFinished-" + player.Nickname, new
                    {
                        result = GameResult.Draw,
                        cell = input.CellNumber,
                        figureType = userGame.FigureType == FigureType.Cross ? 'X' : 'O'
                    });
                }
                else
                {
                    await _gameHub.Clients.All.SendAsync("GameFinished-" + player.Nickname, new
                    {
                        result = GameResult.Draw,
                        cell = input.CellNumber,
                        figureType = userGame.FigureType == FigureType.Cross ? 'O' : 'X'
                    });
                }
            }

            user.CurrentRoom.CurrentGameId = null;
            await _context.SaveChangesAsync();

            return result;
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
    
    public async Task StartGame(string roomName)
    {
        try
        {
            var room = await _context.Rooms
                .Include(r => r.Users)
                .SingleOrDefaultAsync(r => r.Name == roomName);

            if (room == null)
            {
                throw new UserFriendlyException($@"Комната с именем ""{roomName}"" не найдена", -100);
            }

            if (room.Users.Count < 2)
            {
                throw new UserFriendlyException($@"В комнате ""{roomName}"" недостаточно игроков для начала игры", -100);
            }

            var playerFirstId = room.Users.ToArray()[0].Id;
            var playerSecondId = room.Users.ToArray()[1].Id;

            var random = new Random();
            var playerFirstFigureType = random.Next(0, 2);
            
            var game = new Core.Models.Game
            {
                RoomId = room.Id,
                UserTurnId = playerFirstFigureType == 1 ? playerFirstId : playerSecondId
            };

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            await _context.UserGames.AddAsync(new UserGame
            {
                UserId = playerFirstId,
                GameId = game.Id,
                FigureType = playerFirstFigureType == 1 ? FigureType.Cross : FigureType.Nought
            });

            await _context.UserGames.AddAsync(new UserGame
            {
                UserId = playerSecondId,
                GameId = game.Id,
                FigureType = playerFirstFigureType == 1 ? FigureType.Nought : FigureType.Cross
            });

            room.CurrentGameId = game.Id;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw new UserFriendlyException("Не удалось запустить игру", -100);
        }
    }
}