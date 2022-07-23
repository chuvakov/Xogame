using Microsoft.EntityFrameworkCore;
using XOgame.Common.Exceptions;
using XOgame.Core;
using XOgame.Services.Setting.Dto;

namespace XOgame.Services.Setting;

public class SettingService : ISettingService
{
    private readonly XOgameContext _context;
    private readonly ILogger<SettingService> _logger;

    public SettingService(XOgameContext context, ILogger<SettingService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<SettingsDto> GetAll(string nickname)
    {
        var user = await _context.Users
            .Include(u => u.SettingsSound)
            .SingleOrDefaultAsync(u => u.Nickname == nickname);

        if (user == null)
        {
            throw new UserFriendlyException(@$"Пользователь с ником ""{nickname}"" отсутствует");
        }

        var settings = new SettingsDto()
        {
            SoundSettings = new SoundSettingsDto()
            {
                IsEnabledDraw = user.SettingsSound.IsEnabledDraw,
                IsEnabledLose = user.SettingsSound.IsEnabledLose,
                IsEnabledStart = user.SettingsSound.IsEnabledStartGame,
                IsEnabledStep = user.SettingsSound.IsEnabledStep,
                IsEnabledWin = user.SettingsSound.IsEnabledWin
            }
        };

        return settings;
    }

    public async Task Update(UpdateSettingsDto updateSettings)
    {
        var user = await _context.Users
            .Include(u => u.SettingsSound)
            .SingleOrDefaultAsync(u => u.Nickname == updateSettings.Nickname);

        if (user == null)
        {
            throw new UserFriendlyException(@$"Пользователь с ником ""{updateSettings.Nickname}"" отсутствует");
        }

        user.SettingsSound.IsEnabledDraw = updateSettings.Settings.SoundSettings.IsEnabledDraw;
        user.SettingsSound.IsEnabledLose = updateSettings.Settings.SoundSettings.IsEnabledLose;
        user.SettingsSound.IsEnabledStep = updateSettings.Settings.SoundSettings.IsEnabledStep;
        user.SettingsSound.IsEnabledWin = updateSettings.Settings.SoundSettings.IsEnabledWin;
        user.SettingsSound.IsEnabledStartGame = updateSettings.Settings.SoundSettings.IsEnabledStart;

        await _context.SaveChangesAsync();
    }
}