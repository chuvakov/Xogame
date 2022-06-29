using Microsoft.AspNetCore.SignalR;

namespace XOgame.SignalR;

public class GameHub : Hub
{
    public async Task DoStep(string roomName)
    {
        await Clients.Others.SendAsync("DoStep-" + roomName);
    }
}