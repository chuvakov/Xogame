using Microsoft.AspNetCore.SignalR;
using XOgame.SignalR;

namespace XOgame.Tests.Common;

public class NullHubGameContext : IHubContext<GameHub>
{
    public IHubClients Clients { get; }
    public IGroupManager Groups { get; }
}