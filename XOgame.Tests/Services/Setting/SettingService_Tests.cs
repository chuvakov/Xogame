using System.IO;
using System.Linq;
using System.Net;
using DiffEngine;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Moq;
using Shouldly;
using XOgame.Services.Setting;
using XOgame.Services.Setting.Dto;
using Xunit;

namespace XOgame.Tests.Services.Setting;

public class SettingService_Tests : XOgameTestBase
{
    private readonly ISettingService _settingService;

    public SettingService_Tests()
    {
        _settingService = Resolve<ISettingService>();
    }

    [Fact]
    public void GetAll()
    {
        // Arrange
        var nickName = "тимон";
        
        // Act
        var result = _settingService.GetAll(nickName).Result;
        
        // Assert
        result.Avatar.ShouldBeNull();
        result.SoundSettings.IsEnabledDraw.ShouldBeTrue();
        result.SoundSettings.IsEnabledLose.ShouldBeTrue();
        result.SoundSettings.IsEnabledStart.ShouldBeTrue();
        result.SoundSettings.IsEnabledStep.ShouldBeTrue();
        result.SoundSettings.IsEnabledWin.ShouldBeTrue();
    }

    [Fact]
    public void Update()
    {
        // Arrange
        var nickName = "тимон";
        var settings = new SoundSettingsDto()
        {
            IsEnabledDraw = false,
            IsEnabledLose = false,
            IsEnabledStep = false,
            IsEnabledWin = false,
            IsEnabledStart = false,
        };
        
        // Act
        _settingService.Update(new UpdateSettingsDto()
        {
            Nickname = nickName, 
            Settings = new SettingsDto()
            {
                SoundSettings = settings
            }
        }).Wait();
        
        // Assert
        var user = _context.Users
            .Include(u => u.SettingsSound)
            .Single(u => u.Nickname == nickName);
        
        user.SettingsSound.IsEnabledDraw.ShouldBeFalse();
        user.SettingsSound.IsEnabledLose.ShouldBeFalse();
        user.SettingsSound.IsEnabledStep.ShouldBeFalse();
        user.SettingsSound.IsEnabledWin.ShouldBeFalse();
        user.SettingsSound.IsEnabledStartGame.ShouldBeFalse();
    }

    [Fact]
    public void LoadAvatar()
    {
        // Arrange
        var nickName = "тимон";
        var avatarBytes =
            File.ReadAllBytes(
                "/Users/aleksandr/Desktop/Рабочие проеты/Xogame/XOgame.Tests/Services/Setting/Снимок экрана 2022-08-25 в 20.31.29.png");

        var stream = new MemoryStream(avatarBytes);
        var avatar = new FormFile(stream, 0, stream.Length, "avatarName", "fileName");
        
        // Act
        _settingService.LoadAvatar(nickName, avatar).Wait();
        
        // Assert
        var user = _context.Users
            .Single(u => u.Nickname == nickName);
        
        user.PathToAvatar.ShouldNotBeNull();
        File.ReadAllBytes(user.PathToAvatar).ShouldBe(avatarBytes);
    }
}