using XOgame.Core.Models;
using XOgame.Services.Player.Dto;

namespace XOgame.Services.Game.Dto;

public class GameDto
{
    public IEnumerable<PlayerShortDto> Players { get; set; }
    public IEnumerable<StepDto> Steps { get; set; }
}