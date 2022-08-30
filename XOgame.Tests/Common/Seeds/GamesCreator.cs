using System.Collections.Generic;
using XOgame.Core;
using XOgame.Core.Enums;
using XOgame.Core.Models;

namespace XOgame.Tests.Common.Seeds;

public class GamesCreator : CreatorBase
{
    public GamesCreator(XOgameContext context) : base(context)
    {
    }

    public override void Create()
    {
        _context.Games.AddRange
        (
            new Game()
            {
                Id = 1,
                RoomId = 3,
                UserTurnId = 1,
                GameProgresses = new List<GameProgress>()
                {
                    new GameProgress()
                    {
                        GameId = 1,
                        UserId = 1,
                        CellNumber = 1,
                    },
                    new GameProgress()
                    {
                        GameId = 1,
                        UserId = 1,
                        CellNumber = 2,
                    },
                    new GameProgress()
                    {
                        GameId = 1,
                        UserId = 2,
                        CellNumber = 4,
                    }
                }
            },
            new Game()
            {
                Id = 2,
                RoomId = 4,
                UserTurnId = 8,
                GameProgresses = new List<GameProgress>()
            }
        );

        _context.UserGames.AddRange(
            new UserGame()
            {
                UserId = 1,
                GameId = 1,
                FigureType = FigureType.Cross,
            },
            new UserGame()
            {
                UserId = 2,
                GameId = 1,
                FigureType = FigureType.Nought,
            },
            new UserGame()
            {
                UserId = 3,
                GameId = 2,
                FigureType = FigureType.Nought,
            },
            new UserGame()
            {
                UserId = 8,
                GameId = 2,
                FigureType = FigureType.Cross,
            },
            new UserGame()
            {
                UserId = 4,
                GameId = 4,
                FigureType = FigureType.Nought,
            });

        _context.SaveChanges();
    }
}