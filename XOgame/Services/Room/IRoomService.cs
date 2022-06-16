using XOgame.Services.Room.Dto;

namespace XOgame.Services.Room;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAll();
    Task Create(CreateRoomDto dto);
}