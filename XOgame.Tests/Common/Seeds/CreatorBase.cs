using XOgame.Core;

namespace XOgame.Tests.Common.Seeds;

public abstract class CreatorBase
{
    protected readonly XOgameContext _context;

    protected CreatorBase(XOgameContext context)
    {
        _context = context;
    }

    public abstract void Create();
}