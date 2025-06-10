using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using FightingGameServer_Rest.Models.Chatbot;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FightingGameServer_Rest.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize(Policy = "HasPlayer")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class ChatbotController : ControllerBase
{
    private readonly IChatbotWebSocketService _webSocketService;
    private readonly IChatbotGrpcService _grpcService;
    private readonly ILogger<ChatbotController> _logger;

    public ChatbotController(
        IChatbotWebSocketService webSocketService,
        IChatbotGrpcService grpcService,
        ILogger<ChatbotController> logger)
    {
        _webSocketService = webSocketService;
        _grpcService = grpcService;
        _logger = logger;
    }

    /// <summary>
    /// WebSocket 연결을 위한 엔드포인트
    /// </summary>
    [HttpGet("ws")]
    public async Task<IActionResult> WebSocketEndpoint()
    {
        if (!HttpContext.WebSockets.IsWebSocketRequest)
        {
            return BadRequest("WebSocket 요청이 아닙니다.");
        }

        string? playerId = HttpContext.User.FindFirst("playerId")?.Value;
        if (string.IsNullOrEmpty(playerId))
        {
            return Unauthorized("플레이어 ID가 필요합니다.");
        }

        try
        {
            using WebSocket webSocket = await HttpContext.WebSockets.AcceptWebSocketAsync();
            await _webSocketService.HandleWebSocketAsync(playerId, webSocket);
            return Ok();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "WebSocket 연결 처리 중 오류 발생: {PlayerId}", playerId);
            return StatusCode(500, "WebSocket 연결 실패");
        }
    }

    /// <summary>
    /// 세션 초기화 (REST API)
    /// </summary>
    [HttpPost("session")]
    public async Task<ActionResult<InitSessionResponse>> InitSession([FromBody] InitSessionRequest request)
    {
        try
        {
            if (!ChatbotCharacters.IsValidCharacter(request.CharacterRole) ||
                !ChatbotCharacters.IsValidCharacter(request.OpponentRole))
            {
                return BadRequest("유효하지 않은 캐릭터입니다.");
            }

            var response = await _grpcService.InitSessionAsync(request);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "세션 초기화 실패");
            return StatusCode(500, "세션 초기화 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 채팅 메시지 전송 (REST API)
    /// </summary>
    [HttpPost("chat/{sessionId}")]
    public async Task<ActionResult<ChatResponse>> Chat(string sessionId, [FromBody] ChatMessage message)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(message.Message))
            {
                return BadRequest("메시지가 비어있습니다.");
            }

            var response = await _grpcService.ChatAsync(sessionId, message.Message);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "채팅 처리 실패: SessionId {SessionId}", sessionId);
            return StatusCode(500, "채팅 처리 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 게임 상태 분석 (REST API)
    /// </summary>
    [HttpPost("analysis/{sessionId}")]
    public async Task<ActionResult<GameStateAnalysisResponse>> AnalyzeGameState(string sessionId, [FromBody] GameStateAnalysisRequest request)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(request.OpponentActions))
            {
                return BadRequest("분석할 내용이 비어있습니다.");
            }

            var response = await _grpcService.AnalyzeGameStateAsync(sessionId, request.OpponentActions);
            return Ok(response);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "게임 상태 분석 실패: SessionId {SessionId}", sessionId);
            return StatusCode(500, "게임 상태 분석 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 세션 종료 (REST API)
    /// </summary>
    [HttpDelete("session/{sessionId}")]
    public async Task<ActionResult> EndSession(string sessionId)
    {
        try
        {
            bool success = await _grpcService.EndSessionAsync(sessionId);
            
            if (success)
            {
                return Ok(new { Message = "세션이 성공적으로 종료되었습니다." });
            }
            else
            {
                return NotFound("세션을 찾을 수 없습니다.");
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "세션 종료 실패: SessionId {SessionId}", sessionId);
            return StatusCode(500, "세션 종료 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 활성 세션 목록 조회
    /// </summary>
    [HttpGet("sessions")]
    [Authorize(Policy = "Admin")]
    public async Task<ActionResult<List<string>>> GetActiveSessions()
    {
        try
        {
            var sessions = await _grpcService.ListSessionsAsync();
            return Ok(sessions);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "세션 목록 조회 실패");
            return StatusCode(500, "세션 목록 조회 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 현재 플레이어의 세션 정보 조회
    /// </summary>
    [HttpGet("session/current")]
    public async Task<ActionResult<ChatbotSession>> GetCurrentSession()
    {
        try
        {
            string? playerId = HttpContext.User.FindFirst("playerId")?.Value;
            if (string.IsNullOrEmpty(playerId))
            {
                return Unauthorized("플레이어 ID가 필요합니다.");
            }

            var session = await _webSocketService.GetSessionAsync(playerId);
            
            if (session == null)
            {
                return NotFound("활성 세션을 찾을 수 없습니다.");
            }

            return Ok(session);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "현재 세션 조회 실패");
            return StatusCode(500, "세션 조회 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 플레이어 연결 해제
    /// </summary>
    [HttpPost("disconnect")]
    public async Task<ActionResult> Disconnect()
    {
        try
        {
            string? playerId = HttpContext.User.FindFirst("playerId")?.Value;
            if (string.IsNullOrEmpty(playerId))
            {
                return Unauthorized("플레이어 ID가 필요합니다.");
            }

            await _webSocketService.RemovePlayerAsync(playerId);
            return Ok(new { Message = "연결이 해제되었습니다." });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "연결 해제 실패");
            return StatusCode(500, "연결 해제 중 오류가 발생했습니다.");
        }
    }

    /// <summary>
    /// 사용 가능한 캐릭터 목록 조회
    /// </summary>
    [HttpGet("characters")]
    [AllowAnonymous]
    public ActionResult<string[]> GetCharacters()
    {
        return Ok(ChatbotCharacters.ValidCharacters);
    }

    /// <summary>
    /// 서비스 상태 조회
    /// </summary>
    [HttpGet("status")]
    [Authorize(Policy = "Admin")]
    public ActionResult GetStatus()
    {
        try
        {
            int activeConnections = _webSocketService.GetActiveConnectionsCount();
            
            return Ok(new
            {
                Status = "Healthy",
                ActiveConnections = activeConnections,
                Timestamp = DateTime.UtcNow,
                AvailableCharacters = ChatbotCharacters.ValidCharacters
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "상태 조회 실패");
            return StatusCode(500, "상태 조회 중 오류가 발생했습니다.");
        }
    }
}