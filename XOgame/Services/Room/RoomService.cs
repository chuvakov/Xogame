using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Core.Enums;
using XOgame.Core.Models;
using XOgame.Extensions;
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
                MaxAmountUsers = 2,
                IsHavePassword = !string.IsNullOrEmpty(r.Password)
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

            var user = await _context.Users.SingleOrDefaultAsync(u => u.Nickname == input.ManagerNickname);
            if (user == null)
                throw new UserFriendlyException($@"Пользователь с ником ""{input.ManagerNickname}"" не найден", -100);

            if (user.CurrentRoomId.HasValue && user.Role == Role.Manager)
            {
                await ResetRoomManager(user);
            }
            
            var room = new Core.Models.Room
            {
                Name = input.Name,
            };

            if (!string.IsNullOrEmpty(input.Password))
            {
                room.Password = input.Password;
            }
            
            await _context.Rooms.AddAsync(room);
            await _context.SaveChangesAsync();
            
            user.CurrentRoomId = room.Id;
            user.Role = Role.Manager;
            await _context.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
        }
    }

    private async Task ResetRoomManager(User manager)
    {
        var room = await _context.Rooms
            .Include(r => r.Users)
            .SingleAsync(r => r.Id == manager.CurrentRoomId);

        if (room.Users.Any(u => u.Id != manager.Id))
        {
            var user = room.Users.First(u => u.Id != manager.Id);
            user.Role = Role.Manager;
        }
        else
        {
            _context.Rooms.Remove(room);
        }

        await _context.SaveChangesAsync();
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

            if (!string.IsNullOrEmpty(room.Password) && room.Password != input.Password)
            {
                throw new UserFriendlyException("Пароль не верный", -100);
            }

            if (room.Users.Count == 2) throw new UserFriendlyException($"Комната {input.RoomName} заполнена", -100);

            var player = await _context.Users
                .FirstOrDefaultAsync(u => u.Nickname == input.Nickname);

            if (player is null) throw new UserFriendlyException($@"Игрок ""{input.Nickname}"" не найден", -100);

            player.IsReady = false;
            player.CurrentRoomId = room.Id;

            if (room.Users.Any(u => u.Role == Role.Manager && u.Id != player.Id))
            {
                player.Role = Role.Player;
            }
            else
            {
                player.Role = Role.Manager;
            }
            
            await _context.SaveChangesAsync();

            var result = new EnterToGameDto
            {
                Player = new PlayerDto
                {
                    Nickname = player.Nickname,
                    IsReady = player.IsReady,
                    FigureType = FigureType.Cross,
                    Role = player.Role
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
                    IsReady = opponent.IsReady,
                    Role = opponent.Role
                };
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
            else
            {
                if (user.Role == Role.Manager)
                {
                    var secondUser = room.Users.First(u => u.Nickname != user.Nickname);
                    secondUser.Role = Role.Manager;
                }
            }

            user.CurrentRoomId = null;
            user.Role = Role.User;
            await _context.SaveChangesAsync();
            return isRoomDeleted;
        }
        catch (Exception e)
        {
            _logger.Error(e);
            throw;
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
                    Nickname = firstUser.Nickname,
                    Role = firstUser.Role
                }
            };

            if (room.Users.Count == 2)
            {
                var secondUser = room.Users.ToArray()[1];
                
                FigureType? secondUserFigureType = null;
                if (firstUserFigureType.Value == FigureType.Nought)
                {
                    secondUserFigureType = FigureType.Cross;
                }
                else
                {
                    secondUserFigureType = FigureType.Nought;
                }

                players.Add(new PlayerDto
                {
                    FigureType = secondUserFigureType,
                    IsReady = secondUser.IsReady,
                    Nickname = secondUser.Nickname,
                    Role = secondUser.Role
                });
            }

            return new RoomInfoDto()
            {
                Players = players.ToArray(),
                IsGameStarted = room.CurrentGameId.HasValue
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