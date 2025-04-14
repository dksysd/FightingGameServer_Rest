using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.Matchmaking;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class PlayerGraph
{
    private readonly Dictionary<string, Dictionary<string, int>> _graph = new();
    private readonly Dictionary<string, string> _playersSteamIds = new();

    public void AddPlayer(string playerId, string streamId) => _playersSteamIds.Add(playerId, streamId);
    public void RemovePlayer(string playerId) => _playersSteamIds.Remove(playerId);

    public void UpdatePing(string playerId1, string playerId2, int ping)
    {
        if (!_graph.ContainsKey(playerId1)) _graph[playerId1] = new Dictionary<string, int>();
        if (!_graph.ContainsKey(playerId2)) _graph[playerId2] = new Dictionary<string, int>();
        _graph[playerId1][playerId2] = ping;
        _graph[playerId2][playerId1] = ping;
    }

    public Match? FindBestMatch()
    {
        if (_playersSteamIds.Count < 2) return null;

        var bestMatch = _graph
            .SelectMany(kvp => kvp.Value.Select(v => new { Player1 = kvp.Key, Player2 = v.Key, Ping = v.Value }))
            .Where(m => _playersSteamIds.ContainsKey(m.Player1) && _playersSteamIds.ContainsKey(m.Player2))
            .OrderBy(m => m.Ping)
            .FirstOrDefault();

        if (bestMatch == null) return null;

        _playersSteamIds.Remove(bestMatch.Player1);
        _playersSteamIds.Remove(bestMatch.Player2);
        return new Match(bestMatch.Player1, _playersSteamIds[bestMatch.Player1], bestMatch.Player2,
            _playersSteamIds[bestMatch.Player2], bestMatch.Ping);
    }

    public List<KeyValuePair<string, string>> GetWaitingPlayers() => _playersSteamIds.ToList();
}