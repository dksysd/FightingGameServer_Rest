using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Models.Chatbot;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer.Grpc.Generated;
using Grpc.Core;

namespace FightingGameServer_Rest.Services.ApplicationServices;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
public class ChatbotGrpcService(
    CharacterChatService.CharacterChatServiceClient grpcClient,
    ILogger<ChatbotGrpcService> logger)
    : IChatbotGrpcService
{
    public async Task<Models.Chatbot.InitSessionResponse> InitSessionAsync(Models.Chatbot.InitSessionRequest request)
    {
        try
        {
            var grpcRequest = new FightingGameServer.Grpc.Generated.InitSessionRequest
            {
                SessionId = request.SessionId ?? string.Empty,
                CharacterRole = request.CharacterRole,
                OpponentRole = request.OpponentRole,
                Language = request.Language
            };

            var grpcResponse = await grpcClient.InitSessionAsync(grpcRequest);

            return new Models.Chatbot.InitSessionResponse
            {
                Success = grpcResponse.Success,
                SessionId = grpcResponse.SessionId,
                ErrorMessage = grpcResponse.ErrorMessage
            };
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, "gRPC 세션 초기화 실패: {StatusCode}", ex.StatusCode);
            return new Models.Chatbot.InitSessionResponse
            {
                Success = false,
                SessionId = string.Empty,
                ErrorMessage = $"gRPC 연결 오류: {ex.Status.Detail}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "세션 초기화 중 예외 발생");
            return new Models.Chatbot.InitSessionResponse
            {
                Success = false,
                SessionId = string.Empty,
                ErrorMessage = "세션 초기화 실패"
            };
        }
    }

    public async Task<Models.Chatbot.ChatResponse> ChatAsync(string sessionId, string message)
    {
        try
        {
            var grpcRequest = new FightingGameServer.Grpc.Generated.ChatRequest
            {
                SessionId = sessionId,
                UserMessage = message
            };

            var grpcResponse = await grpcClient.ChatAsync(grpcRequest);

            return new Models.Chatbot.ChatResponse
            {
                Speech = grpcResponse.Speech,
                Emotion = grpcResponse.Emotion,
                Success = grpcResponse.Success,
                ErrorMessage = grpcResponse.ErrorMessage,
                Type = ResponseType.Chat
            };
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, "gRPC 채팅 실패: {StatusCode}", ex.StatusCode);
            return new Models.Chatbot.ChatResponse
            {
                Speech = string.Empty,
                Emotion = "당황",
                Success = false,
                ErrorMessage = $"gRPC 연결 오류: {ex.Status.Detail}",
                Type = ResponseType.Error
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "채팅 중 예외 발생");
            return new Models.Chatbot.ChatResponse
            {
                Speech = string.Empty,
                Emotion = "당황",
                Success = false,
                ErrorMessage = "채팅 처리 실패",
                Type = ResponseType.Error
            };
        }
    }

    public async Task<GameStateAnalysisResponse> AnalyzeGameStateAsync(string sessionId, string opponentActions)
    {
        try
        {
            var grpcRequest = new FightingGameServer.Grpc.Generated.AnalysisRequest
            {
                SessionId = sessionId,
                OpponentActions = opponentActions
            };

            var grpcResponse = await grpcClient.AnalyzeGameStateAsync(grpcRequest);

            return new GameStateAnalysisResponse
            {
                Analysis = grpcResponse.Analysis,
                Success = grpcResponse.Success,
                ErrorMessage = grpcResponse.ErrorMessage
            };
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, "gRPC 게임 상태 분석 실패: {StatusCode}", ex.StatusCode);
            return new GameStateAnalysisResponse
            {
                Analysis = string.Empty,
                Success = false,
                ErrorMessage = $"gRPC 연결 오류: {ex.Status.Detail}"
            };
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "게임 상태 분석 중 예외 발생");
            return new GameStateAnalysisResponse
            {
                Analysis = string.Empty,
                Success = false,
                ErrorMessage = "게임 상태 분석 실패"
            };
        }
    }

    public async Task<bool> EndSessionAsync(string sessionId)
    {
        try
        {
            var grpcRequest = new FightingGameServer.Grpc.Generated.EndSessionRequest
            {
                SessionId = sessionId
            };

            var grpcResponse = await grpcClient.EndSessionAsync(grpcRequest);
            return grpcResponse.Success;
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, "gRPC 세션 종료 실패: {StatusCode}", ex.StatusCode);
            return false;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "세션 종료 중 예외 발생");
            return false;
        }
    }

    public async Task<List<string>> ListSessionsAsync()
    {
        try
        {
            var grpcRequest = new FightingGameServer.Grpc.Generated.ListSessionsRequest();
            var grpcResponse = await grpcClient.ListSessionsAsync(grpcRequest);

            return grpcResponse.SessionIds.ToList();
        }
        catch (RpcException ex)
        {
            logger.LogError(ex, "gRPC 세션 목록 조회 실패: {StatusCode}", ex.StatusCode);
            return new List<string>();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "세션 목록 조회 중 예외 발생");
            return new List<string>();
        }
    }
}