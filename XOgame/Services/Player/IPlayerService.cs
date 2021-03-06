namespace XOgame.Services.Player;

public interface IPlayerService
{
    Task<bool> ChangeReady(string nickname);
    Task<char> GetFigureType(string nickname);
    Task<bool> IsPlayerInRoom(string nickname);
}