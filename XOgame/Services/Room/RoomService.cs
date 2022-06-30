using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Core.Enums;
using XOgame.Core.Models;
using XOgame.Extensions;
using XOgame.Services.Player;
using XOgame.Services.Player.Dto;
using XOgame.Services.Room.Dto;

namespace XOgame.Services.Room;

public class RoomService : IRoomService
{
    private readonly XOgameContext _context;
    private readonly ILogger<RoomService> _logger;

    public RoomService(XOgameContext context, ILogger<RoomService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<IEnumerable<RoomDto>> GetAll()
    {
        try
        {
            var rooms = await _context.Rooms
                .Include(r => r.Users)
                .AsNoTracking()
                .ToListAsync();

            var result = rooms.Select(r => new RoomDto
            {
                Name = r.Name,
                AmountUsers = r.Users.Count,
                MaxAmountUsers = 2
            });

            return result;
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    public async Task Create(CreateRoomDto input)
    {
        try
        {
            if (_context.Rooms.Any(r => r.Name == input.Name))
                throw new UserFriendlyException($@"Комната ""{input.Name}"" уже есть", -100);

            await _context.Rooms.AddAsync(new Core.Models.Room
            {
                Name = input.Name
            });

            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    public async Task<EnterToGameDto> Enter(EnterToRoomDto input)
    {
        try
        {
            var room = await _context.Rooms
                .Include(r => r.Users)
                .AsNoTracking() //Не нужно отслеживать сущьность которую ты взял из БД
                .SingleOrDefaultAsync(r => r.Name == input.RoomName);

            if (room is null) throw new UserFriendlyException($@"Комната ""{input.RoomName}"" не найдена", -100);

            if (room.Users.Count == 2) throw new UserFriendlyException($"Комната {input.RoomName} заполнена", -100);

            var player = await _context.Users
                .FirstOrDefaultAsync(u => u.Nickname == input.Nickname);

            if (player is null) throw new UserFriendlyException($@"Игрок ""{input.Nickname}"" не найден", -100);

            player.IsReady = false;
            player.CurrentRoomId = room.Id;
            await _context.SaveChangesAsync();

            var result = new EnterToGameDto
            {
                Player = new PlayerDto
                {
                    Nickname = player.Nickname,
                    IsReady = player.IsReady,
                    FigureType = FigureType.Cross
                }
            };

            if (room.Users.Count == 1)
            {
                result.Player.FigureType = FigureType.Nought;

                var opponent = room.Users.First();
                result.Opponent = new PlayerDto
                {
                    Nickname = opponent.Nickname,
                    FigureType = FigureType.Cross,
                    IsReady = opponent.IsReady
                };

                await StartGame(result.Player, result.Opponent, room.Id);
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    public async Task<bool> Exit(string nickname)
    {
        try
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Nickname == nickname);
            
            if (user == null)
                throw new UserFriendlyException("Пользователь не найден", -100);

            var room = await _context.Rooms
                .Include(r => r.Users)
                .SingleAsync(r => r.Id == user.CurrentRoomId);
            
            var isRoomDeleted = false;
            if (room.Users.Count == 1)
            {
                _context.Rooms.Remove(room);
                isRoomDeleted = true;
            }

            user.CurrentRoomId = null;
            await _context.SaveChangesAsync();
            return isRoomDeleted;
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    //Вспомогательный метод создающий запись в таблице Игра
    private async Task StartGame(PlayerDto playerFirst, PlayerDto playerSecond, int roomId)
    {
        try
        {
            var playerFirstId = await _context.Users
                .Where(u => u.Nickname == playerFirst.Nickname)
                .Select(u => u.Id)
                .FirstAsync();

            var playerSecondId = await _context.Users
                .Where(u => u.Nickname == playerSecond.Nickname)
                .Select(u => u.Id)
                .FirstAsync();
            
            var game = new Core.Models.Game
            {
                RoomId = roomId,
                UserTurnId = playerSecondId
            };

            await _context.Games.AddAsync(game);
            await _context.SaveChangesAsync();

            await _context.UserGames.AddAsync(new UserGame
            {
                UserId = playerFirstId,
                GameId = game.Id,
                FigureType = playerFirst.FigureType.Value
            });

            await _context.UserGames.AddAsync(new UserGame
            {
                UserId = playerSecondId,
                GameId = game.Id,
                FigureType = playerSecond.FigureType.Value
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

    public async Task<RoomInfoDto> GetInfo(string name)
    {
        try
        {
            var room = await _context.Rooms
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Name == name);

            if (room == null)
            {
                throw new UserFriendlyException($"Комната с названием {name} не найдена");
            }

            var firstUser = room.Users.ToArray()[0];
            FigureType? firstUserFigureType = await _context.UserGames
                .Where(ug => ug.UserId == firstUser.Id)
                .Select(ug => ug.FigureType)
                .FirstOrDefaultAsync();

            var players = new List<PlayerDto>()
            {
                new PlayerDto()
                {
                    FigureType = firstUserFigureType,
                    IsReady = firstUser.IsReady,
                    Nickname = firstUser.Nickname
                }
            };

            if (room.Users.Count == 2)
            {
                var secondUser = room.Users.ToArray()[1];
                
                FigureType? secondUserFigureType = null;
                if (firstUserFigureType.HasValue)
                {
                    if (firstUserFigureType.Value == FigureType.Nought)
                    {
                        secondUserFigureType = FigureType.Cross;
                    }
                    else
                    {
                        secondUserFigureType = FigureType.Nought;
                    }
                }

                players.Add(new PlayerDto
                {
                    FigureType = secondUserFigureType,
                    IsReady = secondUser.IsReady,
                    Nickname = secondUser.Nickname
                });
            }

            return new RoomInfoDto()
            {
                Players = players.ToArray(),
            };
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    public async Task Delete(string name)
    {
        try
        {
            var room = await _context.Rooms
                .Include(r => r.Users)
                .FirstOrDefaultAsync(r => r.Name == name);
            
            if (room == null)
            {
                throw new UserFriendlyException(@$"Комната с название ""{name}"" не существует");
            }

            foreach (var user in room.Users)
            {
                user.CurrentRoomId = null;
            }

            _context.Rooms.Remove(room);
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }
}