using System.Diagnostics.CodeAnalysis;
using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using FightingGameServer_Rest.Domains.Matchmaking;
using FightingGameServer_Rest.Domains.Matchmaking.Dto;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.ApplicationServices;

[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class MatchmakingService(
    IMatchRecordService matchRecordService,
    IPlayerService playerService,
    ICharacterService characterService) : IMatchmakingService
{
    private readonly PlayerGraph _graph = new();
    
    private readonly Dictionary<string, WebSocket> _connections = new();

    private readonly Dictionary<string, MatchResult> _matchResults = new();

    public async Task AddPlayerAsync(string playerId, WebSocket webSocket)
    {
        _connections[playerId] = webSocket;
        _graph.AddPlayer(playerId);

        List<string> waitingUsers = _graph.GetWaitingPlayers().Where(u => u != playerId).ToList();
        await SendMessageAsync(playerId, $"PingTest:{JsonSerializer.Serialize(waitingUsers)}");
    }

    public async Task RemovePlayerAsync(string playerId)
    {
        if (_connections.TryGetValue(playerId, out WebSocket? webSocket))
        {
            if (webSocket.State == WebSocketState.Open)
            {
                await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Disconnected", CancellationToken.None);
            }

            _connections.Remove(playerId);
        }

        _graph.RemovePlayer(playerId);
    }

    public async Task ProcessPingResultAsync(string playerId, Dictionary<string, int> pingResults)
    {
        foreach ((string targetPlayerId, int ping) in pingResults)
        {
            _graph.UpdatePing(playerId, targetPlayerId, ping);
        }

        (string playerId1, string playerId2, int ping)? match = _graph.FindBestMatch();
        if (match.HasValue)
        {
            string matchId = Guid.NewGuid().ToString();
            _matchResults[matchId] = new MatchResult
            {
                MatchId = matchId,
                Player1Id = match.Value.playerId1,
                Player2Id = match.Value.playerId2,
                StartedAt = DateTime.UtcNow
            };
            Task task1 = SendMessageAsync(match.Value.playerId1,
                $"Match:{match.Value.playerId2},{match.Value.ping},{matchId}");
            Task task2 = SendMessageAsync(match.Value.playerId2,
                $"Match:{match.Value.playerId1},{match.Value.ping},{matchId}");
            await Task.WhenAll(task1, task2);
        }
    }

    public async Task ProcessMatchResultAsync(string playerId, MatchResultDto matchResultDto)
    {
        if (!_matchResults.TryGetValue(matchResultDto.MatchId, out MatchResult? matchResult))
        {
            await SendMessageAsync(playerId, $"Error: Can't find match for id {matchResultDto.MatchId}");
            return;
        }

        if (matchResult.Player1Id == playerId)
        {
            _matchResults[matchResultDto.MatchId].Player1Result = matchResultDto;
        }
        else if (matchResult.Player2Id == playerId)
        {
            _matchResults[matchResultDto.MatchId].Player2Result = matchResultDto;
        }

        if (_matchResults[matchResultDto.MatchId].Player1Result != null &&
            _matchResults[matchResultDto.MatchId].Player2Result != null)
        {
            MatchResultDto? player1Result = _matchResults[matchResultDto.MatchId].Player1Result;
            MatchResultDto? player2Result = _matchResults[matchResultDto.MatchId].Player2Result;

            if (!player1Result!.Equals(player2Result))
            {
                await Task.WhenAll(
                    SendMessageAsync(matchResult.Player1Id, "Error: Mismatch result"),
                    SendMessageAsync(matchResult.Player2Id, "Error: Mismatch result")
                );
                return;
            }

            Task<Player> winnerPlayerTask = playerService.GetPlayerByName(matchResultDto.WinnerPlayerName);
            Task<Player> loserPlayerTask = playerService.GetPlayerByName(matchResultDto.LoserPlayerName);
            IEnumerable<Character> characters = await characterService.GetAllCharacters();
            List<Character> enumerable = characters.ToList();
            Character winnerCharacter = enumerable.FirstOrDefault(c => c.Name == matchResultDto.WinnerCharacterName) ??
                                        throw new InvalidOperationException(
                                            $"Can't find character {matchResultDto.WinnerCharacterName}");
            Character loserCharacter = enumerable.FirstOrDefault(c => c.Name == matchResultDto.LoserCharacterName) ??
                                       throw new InvalidOperationException(
                                           $"Can't find character {matchResultDto.LoserCharacterName}");
            await Task.WhenAll(winnerPlayerTask, loserPlayerTask);

            await matchRecordService.CreateMatchRecord(new MatchRecord
            {
                StartedAt = matchResult.StartedAt,
                EndedAt = DateTime.UtcNow,
                WinnerPlayerId = winnerPlayerTask.Result.Id,
                WinnerPlayerCharacterId = winnerCharacter.Id,
                LoserPlayerId = loserPlayerTask.Result.Id,
                LoserPlayerCharacterId = loserCharacter.Id
            });

            await Task.WhenAll(
                SendMessageAsync(matchResult.Player1Id, "Result: Confirmed"),
                SendMessageAsync(matchResult.Player2Id, "Result: Confirmed")
            );

            _matchResults.Remove(matchResultDto.MatchId);
        }
    }

    private async Task SendMessageAsync(string playerId, string message)
    {
        if (_connections.TryGetValue(playerId, out WebSocket? socket) && socket.State == WebSocketState.Open)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(message);
            await socket.SendAsync(new ArraySegment<byte>(bytes), WebSocketMessageType.Text, true,
                CancellationToken.None);
        }
    }
}