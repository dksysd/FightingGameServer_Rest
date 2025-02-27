using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Net.WebSockets;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using FightingGameServer_Rest.Authorization;
using FightingGameServer_Rest.Domains.Matchmaking.Dto;
using FightingGameServer_Rest.Services.ApplicationServices;
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
    ILogger<MatchmakingService> logger)
    : ControllerBase
{
    [HttpGet]
    public async Task Get()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
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
        string userId = jwtToken.Claims.First(x => x.Type == ClaimTypes.NameIdentifier).Value;
        string playerId = jwtToken.Claims.First(x => x.Type == "playerId").Value;
        if (userId.IsNullOrEmpty())
        {
            logger.LogWarning("User id is missing");
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        if (playerId.IsNullOrEmpty())
        {
            logger.LogWarning("Player id is missing");
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
        await HandleWebSocket(playerId, webSocket);
    }

    private async Task HandleWebSocket(string playerId, WebSocket webSocket)
    {
        await matchmakingService.AddPlayerAsync(playerId, webSocket);

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