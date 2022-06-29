namespace XOgame.Services.Player;

public interface IPlayerService
{
    Task<bool> ChangeReady(string nickname);
}