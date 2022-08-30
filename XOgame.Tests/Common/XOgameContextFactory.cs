using Microsoft.EntityFrameworkCore;
using XOgame.Core;
using XOgame.Tests.Common.Seeds;

namespace XOgame.Tests.Common;

public class XOgameContextFactory
{
    private static XOgameContext _context;

    public static XOgameContext Context
    {
        get
        {
            if (_context == null)
            {
                _context = Create();
            }

            return _context;
        }
    }
        
    private static XOgameContext Create()
    {
        var options = new DbContextOptionsBuilder<XOgameContext>()
            .UseInMemoryDatabase("XOgameContext")
            .Options;

        var context = new XOgameContext(options);
        context.Database.EnsureCreated();
        
        InitSeeds(context);
        return context;
    }

    public static void Destroy(XOgameContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    private static void InitSeeds(XOgameContext context)
    {
        new EmojisCreator(context).Create();
        new RoomsCreator(context).Create();
        new GamesCreator(context).Create();
        new UsersCreator(context).Create();
    }
}