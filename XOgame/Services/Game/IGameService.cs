using XOgame.Services.Game.Dto;
using XOgame.Services.Player.Dto;

namespace XOgame.Services.Game;

public interface IGameService
{
    Task<DoStepResultDto> DoStep(DoStepInput input);
    Task<GameDto> Get(string roomName);
    Task StartGame(int playerFirstId, int playerSecondId, int roomId);
}