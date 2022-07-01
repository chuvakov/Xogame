using XOgame.Services.Game.Dto;
using XOgame.Services.Player.Dto;

namespace XOgame.Services.Room.Dto;

public class RoomInfoDto
{
    public PlayerDto[] Players { get; set; }
    public bool IsGameStarted { get; set; }
}