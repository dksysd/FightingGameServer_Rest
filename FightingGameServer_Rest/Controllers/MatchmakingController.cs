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
    public async Task<IActionResult> Get()
    {
        WebSocket? webSocket = null;
        string playerId = string.Empty;

        try
        {
            // 1. WebSocket 요청 검증
            if (!HttpContext.WebSockets.IsWebSocketRequest)
            {
                logger.LogWarning("Invalid WebSocket Request");
                return BadRequest("WebSocket 요청이 필요합니다.");
            }
        
            // 2. 필수 매개변수 검증
            if (!TryGetRequiredParameters(out playerId, out string steamId))
            {
                // 오류 응답은 TryGetRequiredParameters 메서드에서 처리됨
                return BadRequest("필수 매개변수가 누락되었습니다.");
            }
        
            // 3. WebSocket 연결 수락
            webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            logger.LogInformation("Web socket request from {PlayerId} accepted.", playerId);
        
            // 4. WebSocket 처리
            await HandleWebSocket(playerId, steamId, webSocket);
            return new EmptyResult(); // WebSocket 처리로 넘어갔으므로 여기서는 응답을 반환하지 않음
        }
        catch (InvalidOperationException operationException)
        {
            logger.LogError(operationException, "매치메이킹 작업 오류");
        
            if (webSocket is { State: WebSocketState.Open })
            {
                await SafeCloseWebSocket(webSocket, WebSocketCloseStatus.InvalidPayloadData, operationException.Message);
                return Conflict(operationException.Message);
            }
            else if (!string.IsNullOrEmpty(playerId))
            {
                await matchmakingService.RemovePlayerAsync(playerId);
                return Conflict(operationException.Message);
            }
            return Conflict();
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        
            // WebSocket이 열려있다면 안전하게 닫음
            if (webSocket is { State: WebSocketState.Open })
            {
                await SafeCloseWebSocket(webSocket, WebSocketCloseStatus.InternalServerError, "Internal server error");
            }
        
            return StatusCode(StatusCodes.Status500InternalServerError);
        }
        finally
        {
            // 추가적인 정리 작업이 필요한 경우
            if (!string.IsNullOrEmpty(playerId) && webSocket?.State != WebSocketState.Open)
            {
                try
                {
                    await matchmakingService.RemovePlayerAsync(playerId);
                }
                catch (Exception ex)
                {
                    logger.LogError(ex.Message);
                }
            }
        }
    }
    
    /// <summary>
    /// 요청에서 필수 매개변수를 가져옵니다.
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="steamId">스팀 ID</param>
    /// <returns>모든 필수 매개변수가 존재하면 true, 그렇지 않으면 false</returns>
    private bool TryGetRequiredParameters(out string playerId, out string steamId)
    {
        playerId = string.Empty;
        steamId = string.Empty;
        
        // 1. 토큰 검증
        StringValues token = HttpContext.Request.Query["websocket_token"];
        if (token.IsNullOrEmpty())
        {
            logger.LogWarning("JwtToken is missing");
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }
    
        // 2. 토큰에서 플레이어 ID 추출
        try
        {
            SecurityToken validatedToken = jwtTokenExtractor.ExtractToken(token!);
            JwtSecurityToken jwtToken = (JwtSecurityToken)validatedToken;
            playerId = jwtToken.Claims.First(x => x.Type == "nameid").Value;
        }
        catch (SecurityTokenExpiredException expiredException)
        {
            logger.LogWarning(expiredException, "JwtToken expired");
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Token validation failed");
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }
        
        if (string.IsNullOrEmpty(playerId))
        {
            logger.LogWarning("Player id is missing");
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }
    
        // 3. 스팀 ID 검증
        StringValues steamIdVal = HttpContext.Request.Query["steam_id"];
        if (steamIdVal.IsNullOrEmpty())
        {
            logger.LogWarning("Steam id is missing");
            HttpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return false;
        }
        steamId = steamIdVal.ToString();
        
        return true;
    }
    
    /// <summary>
    /// WebSocket을 안전하게 닫습니다.
    /// </summary>
    private async Task SafeCloseWebSocket(WebSocket webSocket, WebSocketCloseStatus status, string reason)
    {
        try
        {
            await webSocket.CloseAsync(status, reason, CancellationToken.None);
        }
        catch (Exception closeException)
        {
            logger.LogError($"WebSocket 닫기 오류: {closeException.Message}");
        }
    }

    /// <summary>
    /// 클라이언트와의 WebSocket 통신을 처리합니다.
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="steamId">스팀 ID</param>
    /// <param name="webSocket">열린 WebSocket 연결</param>
    private async Task HandleWebSocket(string playerId, string steamId, WebSocket webSocket)
    {
        // 1. 매치메이킹 서비스에 플레이어 추가
        await matchmakingService.AddPlayerAsync(playerId, steamId, webSocket);

        try
        {
            // 2. 클라이언트로부터 메시지 수신 및 처리
            const int bufferSize = 1024 * 4;
            byte[] buffer = new byte[bufferSize];

            // 연결이 열려있는 동안 메시지 처리
            while (webSocket.State == WebSocketState.Open)
            {
                WebSocketReceiveResult result = await webSocket.ReceiveAsync(
                    new ArraySegment<byte>(buffer),
                    CancellationToken.None
                );

                // 종료 프레임 처리
                if (result.MessageType == WebSocketMessageType.Close)
                {
                    logger.LogInformation($"클라이언트 연결 종료 요청: {playerId}");
                    break;
                }

                // 텍스트 메시지 처리
                if (result.MessageType == WebSocketMessageType.Text)
                {
                    string message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                    await ProcessWebSocketMessage(playerId, message);
                }
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex.Message);
        }
        finally
        {
            // 3. 매치메이킹 서비스에서 플레이어 제거 (정리)
            await matchmakingService.RemovePlayerAsync(playerId);
        }
    }
    
    /// <summary>
    /// WebSocket 메시지를 처리합니다.
    /// </summary>
    /// <param name="playerId">플레이어 ID</param>
    /// <param name="message">메시지 내용</param>
    private async Task ProcessWebSocketMessage(string playerId, string message)
    {
        // PingResult 메시지 처리
        if (message.StartsWith("PingResult:"))
        {
            string jsonPayload = message["PingResult:".Length..];
            
            Dictionary<string, int>? pingResults = 
                JsonSerializer.Deserialize<Dictionary<string, int>>(jsonPayload);
            
            if (pingResults == null)
            {
                throw new InvalidOperationException("Ping result is null");
            }
            
            await matchmakingService.ProcessPingResultAsync(playerId, pingResults);
            return;
        }
        
        // MatchResult 메시지 처리
        if (message.StartsWith("MatchResult:"))
        {
            string jsonPayload = message["MatchResult:".Length..];
            
            MatchResultDto? matchResultDto = 
                JsonSerializer.Deserialize<MatchResultDto>(jsonPayload);
            
            if (matchResultDto == null)
            {
                throw new InvalidOperationException("Match result is null");
            }
            
            await matchmakingService.ProcessMatchResultAsync(playerId, matchResultDto);
            return;
        }
        
        // 알 수 없는 메시지 형식 처리
        logger.LogWarning($"알 수 없는 메시지 형식: {message}");
    }
}