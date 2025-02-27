namespace FightingGameServer_Rest.Domains.Matchmaking.Dto;

public class MatchResultDto
{
    public required string MatchId { get; set; }
    public required string WinnerPlayerName { get; set; }
    public required string WinnerCharacterName { get; set; }
    public required string LoserPlayerName { get; set; }
    public required string LoserCharacterName { get; set; }
}