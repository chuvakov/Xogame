using Microsoft.AspNetCore.Mvc;
using XOgame.Services.Room;
using XOgame.Services.Room.Dto;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }
    
    [HttpGet("[action]")]
    public async Task<IEnumerable<RoomDto>> GetAll()
    {
        return await _roomService.GetAll();
    }

    [HttpPost("[action]")]
    public async Task Create(CreateRoomDto createRoomDto)
    {
        await _roomService.Create(createRoomDto);
    }
}