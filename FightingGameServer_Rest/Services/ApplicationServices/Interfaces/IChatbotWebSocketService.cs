using System.Net.WebSockets;
using FightingGameServer_Rest.Models.Chatbot;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IChatbotWebSocketService
{
    Task HandleWebSocketAsync(string playerId, WebSocket webSocket);
    Task RemovePlayerAsync(string playerId);
    Task<ChatbotSession?> GetSessionAsync(string playerId);
    int GetActiveConnectionsCount();
}