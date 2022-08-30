using System;
using System.Linq;
using Shouldly;
using XOgame.Common.Exceptions;
using XOgame.Services.Player;
using XOgame.Tests.Common;
using Xunit;

namespace XOgame.Tests.Services.Player;

public class PlayerService_Tests : XOgameTestBase
{
    private readonly IPlayerService _playerService;

    public PlayerService_Tests()
    {
        _playerService = Resolve<IPlayerService>();
    }
    
    [Fact]
    public void IsPlayerInRoom_Success()
    {
        // Arrange
        string nickname = "тимон";
        
        // Act
        var result = _playerService.IsPlayerInRoom(nickname).Result;
        
        // Assert
        result.ShouldBe(true);
    }

    [Fact]
    public void IsPlayerInRoom_UsernNotFound()
    {
        // Arrange
        string nickname = "asdff";
        
        // Act
        var exception = Assert.Throws<UserFriendlyException>(() =>
        {
            try
            {
                _playerService.IsPlayerInRoom(nickname).Wait();
                throw new Exception();
            }
            catch (UserFriendlyException e)
            {
                return (e);
            }
        });
        
        // Assert
        exception.Message.ShouldBe($@"Пользователя с никком ""{nickname}"" нет");
    }

    [Fact]
    public void GetFigureType()
    {
        // Arrange
        string nickname = "тимон";
        
        // Act
        var figureType = _playerService.GetFigureType(nickname).Result;
        
        // Assert
        figureType.ShouldBe('X');
    }

    [Fact]
    public void ChangeReady()
    {
        // Arrange
        string nickname = "тимон";
        
        // Act
        var isReady = _playerService.ChangeReady(nickname).Result;
        
        // Assert
        isReady.ShouldBeFalse();
        _context.Users.SingleOrDefault(u => u.Nickname == nickname).IsReady.ShouldBeFalse();
    }
}