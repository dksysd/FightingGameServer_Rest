using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FightingGameServer_Rest.Dtos.CustomCommand;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/customCommand")]
[SuppressMessage("Usage", "CA2254:템플릿은 정적 표현식이어야 합니다.")]
public class CustomCommandController(
    ICustomCommandManageService customCommandManageService,
    ILogger<CustomCommandController> logger) : ControllerBase
{
    [Authorize]
    [HttpGet("all")]
    public async Task<IActionResult> GetCustomCommands()
    {
        try
        {
            string? userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr ?? throw new InvalidOperationException("Invalid user id"));

            IEnumerable<CustomCommandDto> customCommands = await customCommandManageService.GetCustomCommands(userId);
            return Ok(customCommands);
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
    
    [Authorize]
    [HttpPost("set")]
    public async Task<IActionResult> SetCustomCommands(
        [FromBody] IEnumerable<UpdateCustomCommandRequestDto> updateCustomCommandRequestDtos)
    {
        try
        {
            string? userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr ?? throw new InvalidOperationException("Invalid user id"));
            bool isSuccess = await customCommandManageService.SetCustomCommands(updateCustomCommandRequestDtos, userId);
            return isSuccess ? Ok() : Conflict(new { message = "Fail to set custom commands" });
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
}