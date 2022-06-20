using System.Net;
using Microsoft.AspNetCore.Mvc;
using XOgame.Common.Exceptions;
using XOgame.Services.Room;
using XOgame.Services.Room.Dto;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll()
    {
        try
        {
            return Ok(await _roomService.GetAll());
        }
        catch
        {
            return StatusCode((int) HttpStatusCode.InternalServerError, "Не удалось получить информацию о комнатах");
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Create(CreateRoomDto createRoomDto)
    {
        try
        {
            await _roomService.Create(createRoomDto);
            return Ok();
        }
        catch
        {
            return StatusCode((int) HttpStatusCode.InternalServerError, "Не удалось создать комнату");
        }
    }

    [HttpPost("[action]")]
    public async Task<IActionResult> Enter(EnterToRoomDto input)
    {
        try
        {
            return Ok(await _roomService.Enter(input));
        }
        catch (Exception e)
        {
            var message = "Не удалось присоедениться к комнате";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }
    
    [HttpPost("[action]")]
    public async Task<IActionResult> Exit(string nickname)
    {
        try
        {
            await _roomService.Exit(nickname);
            return Ok();
        }
        catch (Exception e)
        {
            var message = "Не удалось выйти из комнаты";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }
}