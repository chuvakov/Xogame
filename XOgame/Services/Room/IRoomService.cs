using XOgame.Services.Room.Dto;

namespace XOgame.Services.Room;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAll();
    Task Create(CreateRoomDto input);
    Task<EnterToGameDto> Enter(EnterToRoomDto input);
    Task Exit(string nickname);
}