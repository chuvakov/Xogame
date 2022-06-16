using Microsoft.EntityFrameworkCore;
using XOgame.Core;
using XOgame.Services.Room.Dto;
using XOgame.Core.Models;

namespace XOgame.Services.Room;

public class RoomService : IRoomService
{
    private readonly XOgameContext _context;

    public RoomService(XOgameContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<RoomDto>> GetAll()
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

    public async Task Create(CreateRoomDto dto)
    {
        await _context.Rooms.AddAsync(new XOgame.Core.Models.Room
        {
            Name = dto.Name
        });

        await _context.SaveChangesAsync();
    }
}