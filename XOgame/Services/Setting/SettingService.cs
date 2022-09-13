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
            },
            Avatar = user.PathToAvatar != null ? File.ReadAllBytes(user.PathToAvatar) : null
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
        
        // LoadAvatar(updateSettings.Nickname, updateSettings.Settings.Avatar)

        await _context.SaveChangesAsync();
    }

    public async Task LoadAvatar(string nickname, IFormFile avatar)
    {
        if (avatar == null)
        {
            return;
        }
        
        var user = await _context.Users.SingleOrDefaultAsync(u => u.Nickname == nickname);
        if (user == null)
        {
            throw new UserFriendlyException($@"Пользователь с ником ""{nickname}"" не найден!");
        }

        if (!Directory.Exists("Avatars"))
        {
            Directory.CreateDirectory("Avatars");
        }
        
        string path = Path.Combine("Avatars", Path.GetFileNameWithoutExtension(avatar.FileName) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + "." + Path.GetExtension(avatar.FileName));
        using (var fs = new FileStream(path, FileMode.Create))
        {
            await avatar.CopyToAsync(fs);
        }

        user.PathToAvatar = path;
        await _context.SaveChangesAsync();
    }
}