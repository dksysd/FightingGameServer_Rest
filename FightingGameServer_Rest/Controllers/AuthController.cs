﻿using System.Diagnostics.CodeAnalysis;
using System.Security.Claims;
using FightingGameServer_Rest.Domains.Auth.Dtos;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/auth")]
[SuppressMessage("Usage", "CA2254:템플릿은 정적 표현식이어야 합니다.")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        try
        {
            await authService.Register(registerRequestDto);
            logger.LogInformation($"Registering new user (id: {registerRequestDto.LoginId})");
            return Ok(new { message = "Registration successful" });
        }
        catch (InvalidOperationException operationException)
        {
            logger.LogWarning(operationException.Message);
            return Conflict(new { message = operationException.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequestDto loginRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            LoginResponseDto loginResponseDto = await authService.Login(loginRequestDto);
            logger.LogInformation($"Login new user (id: {loginRequestDto.LoginId})");
            return Ok(loginResponseDto);
        }
        catch (InvalidOperationException operationException)
        {
            logger.LogWarning(operationException.Message);
            return Conflict(new { message = operationException.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequestDto)
    {
        try
        {
            string? userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr ?? throw new InvalidOperationException("Invalid user id"));

            authService.Logout(logoutRequestDto, userId);
            logger.LogInformation($"Logout user (refresh token : {logoutRequestDto.RefreshToken})");
            return Task.FromResult<IActionResult>(Ok());
        }
        catch (InvalidOperationException operationException)
        {
            logger.LogWarning(operationException.Message);
            return Task.FromResult<IActionResult>(Conflict(new { message = operationException.Message }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal Server Error" }));
        }
    }

    [Authorize]
    [HttpPost("refresh")]
    public Task<IActionResult> Refresh([FromBody] RefreshRequestDto refreshRequestDto)
    {
        try
        {
            string? userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr ?? throw new InvalidOperationException("Invalid user id"));

            RefreshResponseDto refreshResponseDto = authService.Refresh(refreshRequestDto, userId);
            logger.LogInformation($"Refresh user (refresh token : {refreshRequestDto.RefreshToken})");
            return Task.FromResult<IActionResult>(Ok(refreshResponseDto));
        }
        catch (InvalidOperationException operationException)
        {
            logger.LogWarning(operationException.Message);
            return Task.FromResult<IActionResult>(Conflict(new { message = operationException.Message }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal Server Error" }));
        }
    }

    [Authorize]
    [HttpPost("loginPlayer")]
    public async Task<IActionResult> LoginPlayer([FromBody] LoginPlayerRequestDto loginPlayerRequestDto)
    {
        try
        {
            string? userIdStr = HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            int userId = int.Parse(userIdStr ?? throw new InvalidOperationException("Invalid user id"));

            LoginResponseDto loginResponseDto = await authService.LoginPlayer(loginPlayerRequestDto, userId);
            logger.LogInformation($"Login player (refresh token : {loginPlayerRequestDto.RefreshToken})");
            return Ok(loginResponseDto);
        }
        catch (InvalidOperationException operationException)
        {
            logger.LogWarning(operationException.Message);
            return Conflict(new { message = operationException.Message });
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }

    [Authorize]
    [HttpPost("websocket-token")]
    public Task<IActionResult> GetWebSocketToken()
    {
        try
        {
            string playerId = HttpContext.User.FindFirst("playerId")?.Value ??
                              throw new InvalidOperationException("Invalid user id");
            WebSocketTokenResponseDto wsTokenResponseDto = authService.GetWebSocketToken(playerId);
            logger.LogInformation($"Websocket token generated (player id : {playerId})");
            return Task.FromResult<IActionResult>(Ok(wsTokenResponseDto));
        }
        catch (InvalidOperationException operationException)
        {
            logger.LogWarning(operationException.Message);
            return Task.FromResult<IActionResult>(Conflict(new { message = operationException.Message }));
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal Server Error" }));
        }
    }
}