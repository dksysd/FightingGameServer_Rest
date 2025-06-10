using FightingGameServer_Rest.Models.Chatbot;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IChatbotGrpcService
{
    Task<InitSessionResponse> InitSessionAsync(InitSessionRequest request);
    Task<ChatResponse> ChatAsync(string sessionId, string message);
    Task<GameStateAnalysisResponse> AnalyzeGameStateAsync(string sessionId, string opponentActions);
    Task<bool> EndSessionAsync(string sessionId);
    Task<List<string>> ListSessionsAsync();
}