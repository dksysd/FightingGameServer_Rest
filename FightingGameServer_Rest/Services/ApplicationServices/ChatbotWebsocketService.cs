using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using FightingGameServer_Rest.Models.Chatbot;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

namespace FightingGameServer_Rest.Services.ApplicationServices;

[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class ChatbotWebSocketService(IServiceScopeFactory serviceScopeFactory, ILogger<ChatbotWebSocketService> logger)
    : IChatbotWebSocketService
{
    private readonly Dictionary<string, WebSocket> _connections = new();
    private readonly Dictionary<string, ChatbotSession> _sessions = new();
    private readonly object _lock = new();

    public async Task HandleWebSocketAsync(string playerId, WebSocket webSocket)
    {
        lock (_lock)
        {
            _connections[playerId] = webSocket;
        }

        logger.LogInformation("채팅봇 WebSocket 연결 시작: {PlayerId}", playerId);

        try
        {
            // 환영 메시지 전송
            await SendWelcomeMessageAsync(playerId);

            // 메시지 수신 루프
            await ReceiveMessagesAsync(playerId, webSocket);
        }
        catch (WebSocketException ex)
        {
            logger.LogWarning(ex, "WebSocket 예외 발생: {PlayerId}", playerId);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "채팅봇 WebSocket 처리 중 오류: {PlayerId}", playerId);
        }
        finally
        {
            await RemovePlayerAsync(playerId);
        }
    }

    public async Task RemovePlayerAsync(string playerId)
    {
        WebSocket? webSocket = null;
        ChatbotSession? session = null;

        lock (_lock)
        {
            _connections.TryGetValue(playerId, out webSocket);
            _sessions.TryGetValue(playerId, out session);
            _connections.Remove(playerId);
            _sessions.Remove(playerId);
        }

        if (webSocket?.State == WebSocketState.Open)
        {
            try
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "연결 종료", CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "WebSocket 종료 중 오류: {PlayerId}", playerId);
            }
        }

        // gRPC 세션 종료
        if (session != null)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var grpcService = scope.ServiceProvider.GetRequiredService<IChatbotGrpcService>();
                await grpcService.EndSessionAsync(session.SessionId);
                logger.LogInformation("채팅봇 세션 종료: {PlayerId}, SessionId: {SessionId}", playerId, session.SessionId);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "gRPC 세션 종료 실패: {PlayerId}", playerId);
            }
        }
    }

    public Task<ChatbotSession?> GetSessionAsync(string playerId)
    {
        lock (_lock)
        {
            _sessions.TryGetValue(playerId, out ChatbotSession? session);
            return Task.FromResult(session);
        }
    }

    public int GetActiveConnectionsCount()
    {
        lock (_lock)
        {
            return _connections.Count;
        }
    }

    private async Task SendWelcomeMessageAsync(string playerId)
    {
        var welcomeResponse = new WebSocketResponse
        {
            Type = ResponseType.Chat,
            Data = new ChatResponse
            {
                Speech = "안녕하세요! 채팅봇에 오신 것을 환영합니다. 먼저 세션을 초기화해주세요.",
                Emotion = "환영",
                Success = true,
                Type = ResponseType.Chat
            }
        };

        await SendMessageAsync(playerId, welcomeResponse);
    }

    private async Task ReceiveMessagesAsync(string playerId, WebSocket webSocket)
    {
        var buffer = new byte[4096];

        while (webSocket.State == WebSocketState.Open)
        {
            var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

            if (result.MessageType == WebSocketMessageType.Text)
            {
                var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
                await ProcessMessageAsync(playerId, message);
            }
            else if (result.MessageType == WebSocketMessageType.Close)
            {
                break;
            }
        }
    }

    private async Task ProcessMessageAsync(string playerId, string messageContent)
    {
        try
        {
            var message = JsonSerializer.Deserialize<WebSocketMessage>(messageContent, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new System.Text.Json.Serialization.JsonStringEnumConverter() } // Enum 문자열 변환
            });

            if (message == null)
            {
                await SendErrorMessageAsync(playerId, "메시지 형식이 올바르지 않습니다.");
                return;
            }

            WebSocketResponse? response = message.Type switch
            {
                MessageType.InitSession => await HandleInitSessionAsync(playerId, message.Data),
                MessageType.Chat => await HandleChatAsync(playerId, message.Data),
                MessageType.Analysis => await HandleAnalysisAsync(playerId, message.Data),
                MessageType.EndSession => await HandleEndSessionAsync(playerId),
                MessageType.Ping => HandlePingMessage(),
                _ => null
            };

            if (response != null)
            {
                await SendMessageAsync(playerId, response);
            }
        }
        catch (JsonException ex)
        {
            logger.LogWarning(ex, "JSON 파싱 오류: {PlayerId}", playerId);
            await SendErrorMessageAsync(playerId, "메시지 형식이 올바르지 않습니다.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "메시지 처리 중 오류: {PlayerId}", playerId);
            await SendErrorMessageAsync(playerId, "메시지 처리 중 오류가 발생했습니다.");
        }
    }

    private async Task<WebSocketResponse> HandleInitSessionAsync(string playerId, object data)
    {
        try
        {
            var requestJson = JsonSerializer.Serialize(data);
            var request = JsonSerializer.Deserialize<InitSessionRequest>(requestJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (request == null || !ChatbotCharacters.IsValidCharacter(request.CharacterRole) ||
                !ChatbotCharacters.IsValidCharacter(request.OpponentRole))
            {
                return new WebSocketResponse
                {
                    Type = ResponseType.Error,
                    Data = new { ErrorMessage = "잘못된 캐릭터 선택입니다." }
                };
            }

            using var scope = serviceScopeFactory.CreateScope();
            var grpcService = scope.ServiceProvider.GetRequiredService<IChatbotGrpcService>();
            var grpcResponse = await grpcService.InitSessionAsync(request);

            if (grpcResponse.Success)
            {
                var session = new ChatbotSession
                {
                    SessionId = grpcResponse.SessionId,
                    PlayerId = playerId,
                    CharacterRole = request.CharacterRole,
                    OpponentRole = request.OpponentRole,
                    Language = request.Language
                };

                lock (_lock)
                {
                    _sessions[playerId] = session;
                }

                logger.LogInformation("채팅봇 세션 생성: {PlayerId}, SessionId: {SessionId}", playerId, session.SessionId);
            }

            return new WebSocketResponse
            {
                Type = ResponseType.SessionCreated,
                Data = grpcResponse
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "세션 초기화 처리 중 오류: {PlayerId}", playerId);
            return new WebSocketResponse
            {
                Type = ResponseType.Error,
                Data = new { ErrorMessage = "세션 초기화 실패" }
            };
        }
    }

    private async Task<WebSocketResponse?> HandleChatAsync(string playerId, object data)
    {
        ChatbotSession? session;
        lock (_lock)
        {
            _sessions.TryGetValue(playerId, out session);
        }

        if (session == null)
        {
            return new WebSocketResponse
            {
                Type = ResponseType.Error,
                Data = new { ErrorMessage = "세션이 초기화되지 않았습니다." }
            };
        }

        try
        {
            var requestJson = JsonSerializer.Serialize(data);
            var chatMessage = JsonSerializer.Deserialize<ChatMessage>(requestJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            if (chatMessage == null || string.IsNullOrWhiteSpace(chatMessage.Message))
            {
                return new WebSocketResponse
                {
                    Type = ResponseType.Error,
                    Data = new { ErrorMessage = "메시지가 비어있습니다." }
                };
            }

            using var scope = serviceScopeFactory.CreateScope();
            var grpcService = scope.ServiceProvider.GetRequiredService<IChatbotGrpcService>();
            var grpcResponse = await grpcService.ChatAsync(session.SessionId, chatMessage.Message);

            // 세션 활동 시간 업데이트
            lock (_lock)
            {
                if (_sessions.ContainsKey(playerId))
                {
                    _sessions[playerId].LastActivity = DateTime.UtcNow;
                }
            }

            return new WebSocketResponse
            {
                Type = ResponseType.Chat,
                Data = grpcResponse
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "채팅 처리 중 오류: {PlayerId}", playerId);
            return new WebSocketResponse
            {
                Type = ResponseType.Error,
                Data = new { ErrorMessage = "채팅 처리 실패" }
            };
        }
    }

    private async Task<WebSocketResponse?> HandleAnalysisAsync(string playerId, object data)
    {
        ChatbotSession? session;
        lock (_lock)
        {
            _sessions.TryGetValue(playerId, out session);
        }

        if (session == null)
        {
            return new WebSocketResponse
            {
                Type = ResponseType.Error,
                Data = new { ErrorMessage = "세션이 초기화되지 않았습니다." }
            };
        }

        try
        {
            var requestJson = JsonSerializer.Serialize(data);
            var analysisRequest = JsonSerializer.Deserialize<GameStateAnalysisRequest>(requestJson,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

            if (analysisRequest == null || string.IsNullOrWhiteSpace(analysisRequest.OpponentActions))
            {
                return new WebSocketResponse
                {
                    Type = ResponseType.Error,
                    Data = new { ErrorMessage = "분석할 내용이 비어있습니다." }
                };
            }

            using var scope = serviceScopeFactory.CreateScope();
            var grpcService = scope.ServiceProvider.GetRequiredService<IChatbotGrpcService>();
            var grpcResponse =
                await grpcService.AnalyzeGameStateAsync(session.SessionId, analysisRequest.OpponentActions);

            return new WebSocketResponse
            {
                Type = ResponseType.Analysis,
                Data = grpcResponse
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "게임 상태 분석 처리 중 오류: {PlayerId}", playerId);
            return new WebSocketResponse
            {
                Type = ResponseType.Error,
                Data = new { ErrorMessage = "게임 상태 분석 실패" }
            };
        }
    }

    private async Task<WebSocketResponse> HandleEndSessionAsync(string playerId)
    {
        ChatbotSession? session;
        lock (_lock)
        {
            _sessions.TryGetValue(playerId, out session);
            _sessions.Remove(playerId);
        }

        if (session != null)
        {
            try
            {
                using var scope = serviceScopeFactory.CreateScope();
                var grpcService = scope.ServiceProvider.GetRequiredService<IChatbotGrpcService>();
                bool success = await grpcService.EndSessionAsync(session.SessionId);
                logger.LogInformation("채팅봇 세션 종료 요청: {PlayerId}, SessionId: {SessionId}, Success: {Success}",
                    playerId, session.SessionId, success);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "세션 종료 요청 실패: {PlayerId}", playerId);
            }
        }

        return new WebSocketResponse
        {
            Type = ResponseType.SessionEnded,
            Data = new { Success = true, Message = "세션이 종료되었습니다." }
        };
    }

    private WebSocketResponse HandlePingMessage()
    {
        return new WebSocketResponse
        {
            Type = ResponseType.Pong,
            Data = new { Message = "pong" }
        };
    }

    private async Task SendMessageAsync(string playerId, WebSocketResponse response)
    {
        WebSocket? socket;
        lock (_lock)
        {
            _connections.TryGetValue(playerId, out socket);
        }

        if (socket?.State == WebSocketState.Open)
        {
            try
            {
                var json = JsonSerializer.Serialize(response, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                });
                var bytes = Encoding.UTF8.GetBytes(json);
                await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                    CancellationToken.None);
            }
            catch (Exception ex)
            {
                logger.LogWarning(ex, "메시지 전송 실패: {PlayerId}", playerId);
            }
        }
    }

    private async Task SendErrorMessageAsync(string playerId, string errorMessage)
    {
        var errorResponse = new WebSocketResponse
        {
            Type = ResponseType.Error,
            Data = new { ErrorMessage = errorMessage }
        };

        await SendMessageAsync(playerId, errorResponse);
    }
}