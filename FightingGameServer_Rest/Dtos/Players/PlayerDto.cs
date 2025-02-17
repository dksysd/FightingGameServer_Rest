using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Dtos.Players;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class PlayerDto
{
    public required string Name { get; set; }
    public required int ExperiencePoint { get; set; }
    public required int MatchCount { get; set; }

}