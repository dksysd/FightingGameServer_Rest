using FightingGameServer_Rest.Domains.Matchmaking.Dto;

namespace FightingGameServer_Rest.Domains.Matchmaking;

public class MatchResult
{
    public required string MatchId { get; set; }
    public required string Player1Id { get; set; }
    public required string Player2Id { get; set; }
    public required DateTime StartedAt { get; set; }
    public MatchResultDto? Player1Result { get; set; } = null;
    public MatchResultDto? Player2Result { get; set; } = null;
}