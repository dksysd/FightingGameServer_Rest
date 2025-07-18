﻿using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FightingGameServer_Rest.Domains.Player.Dtos;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/player")]
[SuppressMessage("Usage", "CA2254:템플릿은 정적 표현식이어야 합니다.")]
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

            PlayerDto createdPlayerDto =
                await playerInfoService.CreatePlayer(createPlayerRequestDto, userId);
            return Ok(createdPlayerDto);
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
    [HttpGet("list")]
    public async Task<IActionResult> GetAllPlayers()
    {
        try
        {
            string? userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr ?? throw new InvalidOperationException("Invalid user id"));
            List<PlayerDto> players = await playerInfoService.GetAllPlayers(userId);
            return Ok(players);
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
            PlayerDto playerDto = await playerInfoService.GetPlayerInfo(getPlayerInfoRequestDto);
            return Ok(playerDto);
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }
}