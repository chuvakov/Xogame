using XOgame.Services.Game.Dto;
using XOgame.Services.Room.Dto;

namespace XOgame.Services.Room;

public interface IRoomService
{
    Task<IEnumerable<RoomDto>> GetAll(GetAllRoomInput input);
    Task Create(CreateRoomDto input);
    Task<EnterToGameDto> Enter(EnterToRoomDto input);
    Task<bool> Exit(string nickname);
    Task<RoomInfoDto> GetInfo(string name);
    Task Delete(string name);
}