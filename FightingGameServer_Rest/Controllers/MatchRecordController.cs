using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Domains.MatchRecord.Dtos;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/matchRecord")]
[SuppressMessage("Usage", "CA2254:템플릿은 정적 표현식이어야 합니다.")]
public class MatchRecordController(IMatchRecordInfoService matchRecordService, ILogger<MatchRecordController> logger)
    : ControllerBase
{
    [AllowAnonymous]
    [HttpGet]
    public async Task<IActionResult> GetMatchRecords([FromQuery] GetMatchRecordInfoRequestDto request)
    {
        try
        {
            IEnumerable<MatchRecordDto> matchRecords = await matchRecordService.GetMatchRecordInfos(request);
            return Ok(matchRecords);
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