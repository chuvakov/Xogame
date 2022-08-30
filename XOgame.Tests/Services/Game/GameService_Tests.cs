using System.Linq;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using XOgame.Services.Game;
using XOgame.Services.Game.Dto;
using XOgame.Tests.Common.Seeds;
using Xunit;

namespace XOgame.Tests.Services.Game;

public class GameService_Tests : XOgameTestBase
{
    private readonly IGameService _gameService;

    public GameService_Tests()
    {
        _gameService = Resolve<IGameService>();
    }
    
    [Fact]
    public void Get()
    {
        // Arrange
        var roomName = RoomsCreator.ROOM_NAME_WITH_GAME;
        
        // Act
        var game = _gameService.Get(roomName).Result;
        
        // Assert
        game.ShouldNotBeNull();
        game.PlayerTurnNickname.ShouldBe("тимон"); //Todo: Оставить сообщение через метод
        game.Players.Count().ShouldBe(2);
        game.Players.ToList()[0].Nickname.ShouldBe("тимон");
        game.Players.ToList()[1].Nickname.ShouldBe("пумба");
        game.Steps.Count().ShouldBe(3);
    }

    [Fact]
    public void StartGame()
    {
        // Arrange
        var room = "4";
        
        // Act
        _gameService.StartGame(room).Wait();
        
        // Assert
        var curentRoom = _context.Rooms.Single(r => r.Name == room);
        
        curentRoom.CurrentGameId.ShouldNotBeNull();
        var userGames = _context.UserGames.Where(gi => gi.GameId == curentRoom.CurrentGameId);
        userGames.Count().ShouldBe(2);
        userGames.Any(x => x.UserId == 3).ShouldBeTrue();
        userGames.Any(x => x.UserId == 4).ShouldBeTrue();
    }

    [Fact]
    public void DoStep_WithoutFinish()
    {
        // Arrange
        var nickname = "тимон33";
        var cellNumber = 3;
        
        // Act
        var result = _gameService.DoStep(new DoStepInput() {CellNumber = cellNumber, Nickname = nickname}).Result;
        
        // Assert
        result.ShouldNotBeNull();
        result.IsFinish.ShouldBeFalse();
        result.IsWinner.ShouldBeFalse();
        var user = _context.Users
            .Include(r => r.CurrentRoom)
            .ThenInclude(g => g.CurrentGame)
            .ThenInclude(step => step.GameProgresses)
            .Single(u => u.Nickname == nickname);
        user.CurrentRoom.CurrentGame.GameProgresses.Any(gp => gp.UserId == user.Id && gp.CellNumber == cellNumber);
    }

    [Fact]
    public void DoStep_WithFinish()
    {
        // Arrange
        var firstPlayer = "тимон33";
        var secondPlayer = "тимон2";
        _gameService.DoStep(new DoStepInput() {CellNumber = 1, Nickname = firstPlayer});
        _gameService.DoStep(new DoStepInput() {CellNumber = 4, Nickname = secondPlayer});
        _gameService.DoStep(new DoStepInput() {CellNumber = 2, Nickname = firstPlayer});
        _gameService.DoStep(new DoStepInput() {CellNumber = 5, Nickname = secondPlayer});
        
        // Act
        var result = _gameService.DoStep(new DoStepInput() {CellNumber = 3, Nickname = firstPlayer, IsSendNotification = false}).Result;
        
        // Assert
        result.IsFinish.ShouldBeTrue();
        result.IsWinner.ShouldBeTrue();
    }
}