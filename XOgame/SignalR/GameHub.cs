using Microsoft.AspNetCore.SignalR;
using XOgame.Core;

namespace XOgame.SignalR;

public class GameHub : Hub
{
    public async Task DoStep(string roomName, bool isFinish)
    {
        await Clients.Others.SendAsync("DoStep-" + roomName, isFinish);
    }
}