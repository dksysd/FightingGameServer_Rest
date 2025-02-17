using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Dtos.Character;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/character")]
[SuppressMessage("Usage", "CA2254:템플릿은 정적 표현식이어야 합니다.")]
public class CharacterController(ICharacterInfoService characterService, ILogger<CharacterController> logger)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("all")]
    public async Task<IActionResult> GetAllCharacters()
    {
        try
        {
            IEnumerable<CharacterDto> characters = await characterService.GetAllCharacterInfos();
            return Ok(characters);
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