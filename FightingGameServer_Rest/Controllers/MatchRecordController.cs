using FightingGameServer_Rest.Dtos.MatchRecord;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/matchRecord")]
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