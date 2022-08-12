using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using XOgame.Core;

namespace XOgame.SignalR;

public class ChatHub : Hub
{
    private readonly XOgameContext _context;

    public ChatHub(XOgameContext context)
    {
        _context = context;
    }

    public async Task SendMessage(string message, string senderNickname)
    {
        var room = await _context
            .Users
            .Include(u => u.CurrentRoom)
            .ThenInclude(r => r.Users)
            .Where(u => u.Nickname == senderNickname)
            .Select(u => u.CurrentRoom)
            .SingleOrDefaultAsync();

        var opponent = room.Users.Single(u => u.Nickname != senderNickname);
        await Clients.Others.SendAsync($"SendMessage-{opponent.Nickname}", message);
    }
}