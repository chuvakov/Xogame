using XOgame.Services.Game.Dto;

namespace XOgame.Services.Game;

public interface IGameService
{
    Task<DoStepResultDto> DoStep(DoStepInput input);
    Task<GameDto> Get(string roomName);
}