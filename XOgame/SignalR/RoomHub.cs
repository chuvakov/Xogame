using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Services.Player.Dto;

namespace XOgame.SignalR;

public class RoomHub : Hub
{
    private readonly XOgameContext _context;

    public RoomHub(XOgameContext context)
    {
        _context = context;
    }

    public async Task CreateRoom(string name)
    {
        var room = await _context.Rooms
            .SingleOrDefaultAsync(r => r.Name == name);

        if (room == null)
        {
            throw new UserFriendlyException(@$"Комната с названием ""{name}"" не найдена, -100");
        }

        await Clients.All.SendAsync("CreateRoom");
    }

    public async Task ChangeStateRoom(string name)
    {
        var players = new List<PlayerDto>();
        var room = await _context.Rooms
            .Include(u => u.Users)
            .SingleOrDefaultAsync(r => r.Name == name);

        if (room == null)
        {
            return;
        }

        foreach (var user in room.Users)
        {
            var player = new PlayerDto()
            {
                Nickname = user.Nickname,
                IsReady = user.IsReady,
                Role = user.Role
            };

            if (room.CurrentGameId != null)
            {
                var userGame = await _context.UserGames
                    .SingleAsync(ug => ug.UserId == user.Id && ug.GameId == room.CurrentGameId);

                player.FigureType = userGame.FigureType;
            }
            
            players.Add(player);
        }

        await Clients.All.SendAsync("ChangeStateRoom" + name, players);
    }

    public async Task StartGame(string roomName)
    {
        var room = await _context.Rooms.SingleOrDefaultAsync(r => r.Name == roomName);

        if (room == null)
        {
            throw new UserFriendlyException(@$"Комната с названием ""{roomName}"" не найдена, -100");
        }

        await Clients.All.SendAsync("StartGame" + roomName);
    }
}