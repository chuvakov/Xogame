using System.Linq;
using Shouldly;
using XOgame.Core.Enums;
using XOgame.Core.Models;
using XOgame.Services.Room;
using XOgame.Services.Room.Dto;
using XOgame.Tests.Common;
using XOgame.Tests.Common.Seeds;
using Xunit;

namespace XOgame.Tests.Services.Room;

public class RoomService_Tests : XOgameTestBase
{
    private readonly IRoomService _roomService;

    public RoomService_Tests()
    {
        _roomService = Resolve<IRoomService>();
    }

    [Fact]
    public void GetAll_All()
    {
        // Arrange
        var roomCount = _context.Rooms.Count();
        var input = new GetAllRoomInput();
        
        // Act
        var result = _roomService.GetAll(input).Result;
        
        // Assert
        result.ShouldNotBeNull();
        result.Count().ShouldBe(roomCount);
    }

    [Fact]
    public void GetAll_ByKeywords()
    {
        // Arrange
        var keyword = "s";
        var roomCount = _context.Rooms.Where(r => r.Name.Contains(keyword)).Count();
        var input = new GetAllRoomInput(keyword);

        // Act
        var result = _roomService.GetAll(input).Result;
        
        // Assert
        result.Count().ShouldBe(roomCount);
    }

    [Fact]
    public void Create()
    {
        // Arrange
        var input = new CreateRoomDto()
        {
            ManagerNickname = "тимон",
            Name = "комната33"
        };
        
        // Act
        _roomService.Create(input).Wait();
        
        // Assert
        _context.Rooms.Count().ShouldBe(6);
        var room = _context.Rooms.Single(r => r.Name == input.Name);
        var manager = _context.Users.Single(u => u.Nickname == input.ManagerNickname);
        
        manager.Role.ShouldBe(Role.Manager);
        manager.CurrentRoomId.ShouldBe(room.Id);
    }

    [Fact]
    public void Create_ResetRoomManager_WithAnyUser()
    {
        // Arrange
        var nickname = "тимон2";
        var input = new CreateRoomDto()
        {
            ManagerNickname = nickname,
            Name = "комната33"
        };
        
        // Act
        _roomService.Create(input).Wait();
        
        // Assert
        _context.Rooms.Count().ShouldBe(6);
        var room = _context.Rooms.Single(r => r.Name == input.Name);
        var manager = _context.Users.Single(u => u.Nickname == input.ManagerNickname);
        
        manager.Role.ShouldBe(Role.Manager);
        manager.CurrentRoomId.ShouldBe(room.Id);

        var user = _context.Users.Single(u => u.Nickname == "тимон3" && u.CurrentRoomId == 4);
        user.Role.ShouldBe(Role.Manager);
    }

    [Fact]
    public void Create_ResetRoomManager_WithoutAnyUser()
    {
        // Arrange
        var nickname = "тимон4";
        var input = new CreateRoomDto()
        {
            ManagerNickname = nickname,
            Name = "комната33"
        };
        
        // Act
        _roomService.Create(input).Wait();
        
        // Assert
        _context.Rooms.Count().ShouldBe(5);
        var room = _context.Rooms.Single(r => r.Name == input.Name);
        var manager = _context.Users.Single(u => u.Nickname == input.ManagerNickname);
        
        manager.Role.ShouldBe(Role.Manager);
        manager.CurrentRoomId.ShouldBe(room.Id);
        _context.Rooms.FirstOrDefault(r => r.Name == "5").ShouldBeNull();
    }

    [Fact]
    public void Enter()
    {
        // Arrange
        var room = _context.Rooms.Single(r => r.Name == "5");
        var user = new User()
        {
            Nickname = "Пумба2",
            Role = Role.User,
        };

        _context.Users.Add(user);
        _context.SaveChanges();

        // Act
        var result = _roomService.Enter(new EnterToRoomDto
        {
            Nickname = user.Nickname,
            RoomName = room.Name,
            Password = "1234",
        }).Result;
        
        // Assert
        user.CurrentRoomId.ShouldBe(room.Id);
        result.Player.Nickname.ShouldBe(user.Nickname);
        result.Opponent.Nickname.ShouldBe("тимон4");
    }

    [Fact]
    public void Exit_Player()
    {
        // Arrange
        var nickname = "тимон3";
        
        // Act
        var result = _roomService.Exit(nickname).Result;
        
        // Assert
        result.ShouldBe(false);
        var user = _context.Users.Single(u => u.Nickname == nickname);
        user.CurrentRoomId.ShouldBeNull();
        user.Role.ShouldBe(Role.User);
    }
    
    [Fact]
    public void Exit_Manager()
    {
        // Arrange
        var nickname = "тимон2";
        
        // Act
        var result = _roomService.Exit(nickname).Result;
        
        // Assert
        result.ShouldBe(false);
        var user = _context.Users.Single(u => u.Nickname == nickname);
        user.Role.ShouldBe(Role.User);
        user.CurrentRoomId.ShouldBeNull();
        
        _context.Users.Single(u => u.Nickname == "тимон3").Role.ShouldBe(Role.Manager);
    }
    
    [Fact]
    public void Exit_SoloPlayer()
    {
        // Arrange
        var nickname = "тимон4";
        
        // Act
        var result = _roomService.Exit(nickname).Result;
        
        // Assert
        result.ShouldBe(true);
        var user = _context.Users.Single(u => u.Nickname == nickname);
        user.Role.ShouldBe(Role.User);
        user.CurrentRoomId.ShouldBeNull();
    }

    [Fact]
    public void GetInfo_RoomWithGame()
    {
        // Arrange
        var roomName = RoomsCreator.ROOM_NAME_WITH_GAME;
        
        // Act
        var result = _roomService.GetInfo(roomName).Result;
        
        // Assert
        result.Players.Length.ShouldBe(2);
        result.Players[0].Nickname.ShouldBe("тимон");
        result.Players[1].Nickname.ShouldBe("пумба");
        result.Players[0].Role.ShouldBe(Role.Player);
        result.Players[1].Role.ShouldBe(Role.Manager);
        result.Players[0].FigureType.ShouldBe(FigureType.Cross);
        result.Players[1].FigureType.ShouldBe(FigureType.Nought);
        result.IsGameStarted.ShouldBeTrue();
    }

    [Fact]
    public void GetInfo_RoomWithoutGame()
    {
        // Arrange
        var roomName = "5";
        
        // Act
        var result = _roomService.GetInfo(roomName).Result;
        
        // Assert
        result.Players.Length.ShouldBe(1);
        result.Players[0].Nickname.ShouldBe("тимон4");
        result.Players[0].Role.ShouldBe(Role.Manager);
        result.Players[0].FigureType.ShouldBeNull();
        result.IsGameStarted.ShouldBeFalse();
    }
    
    [Fact]
    public void GetInfo_RoomWithoutGame_TwoPlayers()
    {
        // Arrange
        var roomName = "6";
        
        // Act
        var result = _roomService.GetInfo(roomName).Result;
        
        // Assert
        result.Players.Length.ShouldBe(2);
        result.Players[0].Nickname.ShouldBe("тимон77");
        result.Players[1].Nickname.ShouldBe("тимон88");
        result.Players[0].Role.ShouldBe(Role.Manager);
        result.Players[1].Role.ShouldBe(Role.Player);
        result.Players[0].FigureType.ShouldBeNull();
        result.Players[1].FigureType.ShouldBeNull();
        result.IsGameStarted.ShouldBeFalse();
    }

    [Fact]
    public void Delete()
    {
        // Arrange
        var roomName = "5";
        
        // Act 
        _roomService.Delete(roomName).Wait();
        
        // Assert
        _context.Rooms.SingleOrDefault(r => r.Name == roomName).ShouldBeNull();
    }
}