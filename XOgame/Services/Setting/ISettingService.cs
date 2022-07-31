using XOgame.Services.Setting.Dto;

namespace XOgame.Services.Setting;

public interface ISettingService
{
    Task<SettingsDto> GetAll(string userName);
    Task Update(UpdateSettingsDto updateSettings);
    Task LoadAvatar(string nickname, IFormFile avatar);
}