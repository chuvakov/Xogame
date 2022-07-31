using Microsoft.AspNetCore.Mvc;
using XOgame.Services.Setting;
using XOgame.Services.Setting.Dto;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly ISettingService _service;

    public SettingsController(ISettingService service)
    {
        _service = service;
    }
    
    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll(string nickname)
    {
        return Ok(await _service.GetAll(nickname));
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> Update(UpdateSettingsDto input)
    {
        await _service.Update(input);
        return Ok();
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> LoadAvatar(string nickname, [FromForm]IFormFile avatar)
    {
        await _service.LoadAvatar(nickname, avatar);
        return Ok();
    }
}