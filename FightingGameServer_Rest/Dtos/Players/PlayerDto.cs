namespace FightingGameServer_Rest.Dtos.Players;

public class PlayerDto
{
    public required string Name { get; set; }
    public required int ExperiencePoint { get; set; }
    public required int MatchCount { get; set; }

}