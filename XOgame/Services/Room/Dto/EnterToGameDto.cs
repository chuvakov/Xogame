using XOgame.Services.Player.Dto;

namespace XOgame.Services.Room.Dto;

public class EnterToGameDto
{
    public PlayerDto Player { get; set; }
    public PlayerDto Opponent { get; set; }
}