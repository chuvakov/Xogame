using XOgame.Core;
using XOgame.Core.Enums;
using XOgame.Core.Models;

namespace XOgame.Tests.Common.Seeds;

public class UsersCreator : CreatorBase
{
    public UsersCreator(XOgameContext context) : base(context)
    {
    }

    public override void Create()
    {
        _context.Users.AddRange
        (
            new User()
            {
                Id = 1,
                CurrentRoomId = 3,
                IsReady = true,
                Nickname = "тимон",
                Password = "123",
                Role = Role.Player,
                SettingsSound = new SettingsSound()
                {
                    IsEnabledDraw = true,
                    IsEnabledLose = true,
                    IsEnabledStep = true,
                    IsEnabledWin = true,
                    IsEnabledStartGame = true
                },
            },
            new User()
            {
                Id = 2,
                CurrentRoomId = 3,
                IsReady = true,
                Nickname = "пумба",
                Password = "123",
                Role = Role.Manager
            },
            new User()
            {
                Id = 3,
                CurrentRoomId = 4,
                IsReady = true,
                Nickname = "тимон2",
                Password = "123",
                Role = Role.Manager
            },
            new User()
            {
                Id = 8,
                CurrentRoomId = 4,
                IsReady = true,
                Nickname = "тимон33",
                Password = "123",
                Role = Role.Player
            },
            new User()
            {
                Id = 4,
                CurrentRoomId = 3,
                IsReady = true,
                Nickname = "тимон3",
                Password = "123",
                Role = Role.Player
            },
            new User()
            {
                Id = 5,
                CurrentRoomId = 5,
                IsReady = true,
                Nickname = "тимон4",
                Password = "123",
                Role = Role.Manager
            },
            new User()
            {
                Id = 6,
                CurrentRoomId = 6,
                IsReady = true,
                Nickname = "тимон77",
                Password = "123",
                Role = Role.Manager
            },
            new User()
            {
                Id = 7,
                CurrentRoomId = 6,
                IsReady = false,
                Nickname = "тимон88",
                Password = "123",
                Role = Role.Player
            }
        );

        _context.SaveChanges();
    }
}