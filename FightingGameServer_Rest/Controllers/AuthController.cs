using FightingGameServer_Rest.Dtos;
using FightingGameServer_Rest.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController(IAuthService authService) : ControllerBase
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
        catch (Exception)
        {
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
            string sessionToken = await authService.Login(loginRequestDto);
            return Ok(new { sessionToken });
        }
        catch (InvalidOperationException operationException)
        {
            return Conflict(new { message = operationException.Message });
        }
        catch (Exception)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Internal Server Error" });
        }
    }

    [Authorize]
    [HttpPost("logout")]
    public Task<IActionResult> Logout()
    {
        try
        {
            authService.Logout(Request.Headers["session-token"]!);
            return Task.FromResult<IActionResult>(Ok());
        }
        catch (InvalidOperationException operationException)
        {
            return Task.FromResult<IActionResult>(Conflict(new { message = operationException.Message }));
        }
        catch (Exception)
        {
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal Server Error" }));
        }
    }

    [Authorize]
    [HttpPost("heartbeat")]
    public Task<IActionResult> Heartbeat()
    {
        try
        {
            authService.Heartbeat(Request.Headers["session-token"]!);
            return Task.FromResult<IActionResult>(Ok());
        }
        catch (InvalidOperationException operationException)
        {
            return Task.FromResult<IActionResult>(Conflict(new { message = operationException.Message }));
        }
        catch (Exception)
        {
            return Task.FromResult<IActionResult>(StatusCode(StatusCodes.Status500InternalServerError,
                new { message = "Internal Server Error" }));
        }
    }
}