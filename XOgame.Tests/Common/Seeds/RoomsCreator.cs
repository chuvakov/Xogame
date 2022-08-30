using XOgame.Core;
using XOgame.Core.Models;

namespace XOgame.Tests.Common.Seeds;

public class RoomsCreator : CreatorBase
{

    public const string ROOM_NAME_WITH_GAME = "RoomWithGame";
    
    public RoomsCreator(XOgameContext context) : base(context)
    {
    }

    public override void Create()
    {
        _context.Rooms.AddRange
        (
            new Room
            {
                Id = 1,
                Name = "1s"
            }, 
            new Room
            {
                Id = 2,
                Name = "2s",
                Password = "1234"
            },
            new Room
            {
                Id = 3,
                Name = ROOM_NAME_WITH_GAME,
                Password = "1234",
                CurrentGameId = 1,
            },
            new Room
            {
                Id = 4,
                Name = "4",
                Password = "1234",
                CurrentGameId = 2,
            },
            new Room
            {
                Id = 5,
                Name = "5",
                Password = "1234",
            },
            new Room
            {
                Id = 6,
                Name = "6",
            }
        );

        _context.SaveChanges();
    }
}