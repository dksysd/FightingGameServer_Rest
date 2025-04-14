using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FightingGameServer_Rest.Authorization;
using FightingGameServer_Rest.Domains.Matchmaking.Dto;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/matchmaking")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class MatchmakingController(
    IMatchmakingService matchmakingService,
    JwtTokenExtractor jwtTokenExtractor,
    ILogger<MatchmakingController> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task Get()
    {
        try
        {
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                logger.LogWarning("Invalid WebSocket Request");
                HttpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
                return;
            }

            StringValues token = HttpContext.Request.Query["websocket_token"];
            if (token.IsNullOrEmpty())
            {
                logger.LogWarning("JwtToken is missing");
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            SecurityToken validatedToken;
            try
            {
                validatedToken = jwtTokenExtractor.ExtractToken(token!);
            }
            catch (SecurityTokenExpiredException expiredException)
            {
                logger.LogWarning(expiredException, "JwtToken expired");
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            string playerId = jwtToken.Claims.First(x => x.Type == "nameid").Value;
            if (playerId.IsNullOrEmpty())
            {
                logger.LogWarning("Player id is missing");
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            StringValues steamId = HttpContext.Request.Query["steam_id"];
            if (steamId.IsNullOrEmpty())
            {
                logger.LogWarning("Steam id is missing");
                HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
                return;
            }

            WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();

            try
            {
                await HandleWebSocket(playerId, steamId.ToString(), webSocket);
            }
            catch (InvalidOperationException operationException)
            {
                logger.LogWarning(operationException.Message);
                if (webSocket.State == WebSocketState.Open) HttpContext.Response.StatusCode = StatusCodes.Status409Conflict;
                else await matchmakingService.RemovePlayerAsync(playerId);
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
            HttpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
        }
    }

    private async Task HandleWebSocket(string playerId, string steamId, WebSocket webSocket)
    {
        await matchmakingService.AddPlayerAsync(playerId, steamId, webSocket);

        byte[] buffer = new byte[1024 * 4];
        while (webSocket.State == WebSocketState.Open)
        {
            WebSocketReceiveResult result =
                await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            string message = Encoding.UTF8.GetString(buffer, 0, result.Count);

            if (message.StartsWith("PingResult:"))
            {
                Dictionary<string, int> pingResults =
                    JsonSerializer.Deserialize<Dictionary<string, int>>(message["PingResult:".Length..]) ??
                    throw new InvalidOperationException("Ping result is null");
                await matchmakingService.ProcessPingResultAsync(playerId, pingResults);
            }
            else if (message.StartsWith("MatchResult:"))
            {
                MatchResultDto matchResultDto =
                    JsonSerializer.Deserialize<MatchResultDto>(message["MatchResult:".Length..]) ??
                    throw new InvalidOperationException("Match result is null");
                await matchmakingService.ProcessMatchResultAsync(playerId, matchResultDto);
            }
        }

        await matchmakingService.RemovePlayerAsync(playerId);
    }
}