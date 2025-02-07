namespace FightingGameServer_Rest.Dtos.Player;

public class GetPlayerInfoResponseDto
{
    public required string Name { get; set; }
    public required int ExperiencePoint { get; set; }
    public required int MatchCount { get; set; }
}