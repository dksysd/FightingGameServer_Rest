using FightingGameServer_Rest.Dtos.Auth;
using FightingGameServer_Rest.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService, ILogger<AuthController> logger) : ControllerBase
{
    [AllowAnonymous]
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequestDto registerRequestDto)
    {
        if (!ModelState.IsValid)
        {
            return BadRequest(ModelState);
        }

        try
        {
            await authService.Register(registerRequestDto);
            return Ok(new { message = "Registration successful" });
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
            return Ok(loginResponseDto);
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
    [HttpPost("logout")]
    public Task<IActionResult> Logout([FromBody] LogoutRequestDto logoutRequestDto)
    {
        try
        {
            authService.Logout(logoutRequestDto);
            return Task.FromResult<IActionResult>(Ok());
        }
        catch (InvalidOperationException operationException)
        {
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
            RefreshResponseDto refreshResponseDto = authService.Refresh(refreshRequestDto);
            return Task.FromResult<IActionResult>(Ok(refreshResponseDto));
        }
        catch (InvalidOperationException operationException)
        {
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