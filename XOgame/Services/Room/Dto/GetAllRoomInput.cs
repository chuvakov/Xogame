using XOgame.Common.Dto;

namespace XOgame.Services.Room.Dto;

public class GetAllRoomInput : IFilteredResultRequest
{
    public string Keyword { get; set; }
}