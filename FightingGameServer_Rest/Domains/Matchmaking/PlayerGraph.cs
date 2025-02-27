using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.Matchmaking;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class PlayerGraph
{
    private readonly Dictionary<string, Dictionary<string, int>> _graph = new();
    private readonly HashSet<string> _players = [];

    public void AddPlayer(string playerId) => _players.Add(playerId);
    public void RemovePlayer(string playerId) => _players.Remove(playerId);

    public void UpdatePing(string playerId1, string playerId2, int ping)
    {
        if (!_graph.ContainsKey(playerId1)) _graph[playerId1] = new Dictionary<string, int>();
        if (!_graph.ContainsKey(playerId2)) _graph[playerId2] = new Dictionary<string, int>();
        _graph[playerId1][playerId2] = ping;
        _graph[playerId2][playerId1] = ping;
    }

    public (string playerId1, string playerId2, int ping)? FindBestMatch()
    {
        if (_players.Count < 2) return null;

        var bestMatch = _graph
            .SelectMany(kvp => kvp.Value.Select(v => new { Player1 = kvp.Key, Player2 = v.Key, Ping = v.Value }))
            .Where(m => _players.Contains(m.Player1) && _players.Contains(m.Player2))
            .OrderBy(m => m.Ping)
            .FirstOrDefault();

        if (bestMatch == null) return null;

        _players.Remove(bestMatch.Player1);
        _players.Remove(bestMatch.Player2);
        return (bestMatch.Player1, bestMatch.Player2, bestMatch.Ping);
    }
    
    public List<string> GetWaitingPlayers() => _players.ToList();
}