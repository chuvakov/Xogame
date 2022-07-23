using System.Net;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using XOgame.Common.Exceptions;
using XOgame.Extensions;
using XOgame.Services.Player;
using XOgame.Services.Room;
using XOgame.Services.Room.Dto;
using XOgame.SignalR;

namespace XOgame.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;
    private readonly IHubContext<RoomHub> _roomHub;
    private readonly IPlayerService _playerService;
    private readonly ILogger<RoomsController> _logger;

    public RoomsController(IRoomService roomService, IHubContext<RoomHub> roomHub, IPlayerService playerService, ILogger<RoomsController> logger)
    {
        _roomService = roomService;
        _roomHub = roomHub;
        _playerService = playerService;
        _logger = logger;
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetAll([FromQuery] GetAllRoomInput input)
    {
        try
        {
            return Ok(await _roomService.GetAll(input));
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
            if (await _playerService.IsPlayerInRoom(input.Nickname))
            {
                _logger.Error(@$"Пользователь с никком ""{input.Nickname}"" уже находится в другой комнате");
                return BadRequest("Ник занят");
            }
            return Ok(await _roomService.Enter(input));
        }
        catch (Exception e)
        {
            _logger.Error(e);
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
            var isRoomDeleted = await _roomService.Exit(nickname);
            if (isRoomDeleted)
            {
                await _roomHub.Clients.All.SendAsync("DeleteRoom");
            }
            return Ok();
        }
        catch (Exception e)
        {
            var message = "Не удалось выйти из комнаты";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }

    [HttpGet("[action]")]
    public async Task<IActionResult> GetInfo(string name)
    {
        try
        {
            return Ok(await _roomService.GetInfo(name));
        }
        catch (Exception e)
        {
            var message = @$"Не удалось получить информацию о комнате ""{name}""";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }

    [HttpDelete("[action]")]
    public async Task<IActionResult> Delete(string name)
    {
        try
        {
            await _roomService.Delete(name);
            return Ok();
        }
        catch (Exception e)
        {
            var message = @$"Не удалось удалить комнату ""{name}""";

            if (e is UserFriendlyException) message = e.Message;
            return StatusCode((int) HttpStatusCode.InternalServerError, message);
        }
    }
}