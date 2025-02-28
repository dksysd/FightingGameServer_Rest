using System.Net.WebSockets;
using FightingGameServer_Rest.Domains.Matchmaking.Dto;
using FightingGameServer_Rest.Domains.MatchRecord.Dtos;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IMatchmakingService
{
    Task AddPlayerAsync(string playerId, WebSocket webSocket);
    Task RemovePlayerAsync(string playerId);
    Task ProcessPingResultAsync(string playerId, Dictionary<string, int> pingResults);
    Task ProcessMatchResultAsync(string playerId, MatchResultDto matchResultDto);
}