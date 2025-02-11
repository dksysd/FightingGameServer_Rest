using System.Security.Claims;
using FightingGameServer_Rest.Dtos.Player;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/player")]
public class PlayerController(IPlayerInfoService playerInfoService, ILogger<PlayerController> logger) : ControllerBase
{
    [Authorize]
    [HttpPost("create")]
    public async Task<IActionResult> CreatePlayer([FromBody] CreatePlayerRequestDto createPlayerRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            string? userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr ?? throw new InvalidOperationException("Invalid user id"));

            CreatePlayerResponseDto createPlayerResponseDto =
                await playerInfoService.CreatePlayer(createPlayerRequestDto, userId);
            return Ok(createPlayerResponseDto);
        }
        catch (InvalidOperationException operationException)
        {
            return Conflict(new { message = operationException.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }

    [AllowAnonymous]
    [HttpGet("get")]
    public async Task<IActionResult> GetPlayerInfo([FromQuery] GetPlayerInfoRequestDto getPlayerInfoRequestDto)
    {
        try
        {
            GetPlayerInfoResponseDto playerInfo = await playerInfoService.GetPlayerInfo(getPlayerInfoRequestDto);
            return Ok(playerInfo);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }
}