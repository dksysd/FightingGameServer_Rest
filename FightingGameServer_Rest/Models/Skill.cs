namespace FightingGameServer_Rest.Models;

public sealed class Skill
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required bool IsPassive { get; init; }
    public required int CoolTime { get; init; }
    public required int Range { get; init; }
    public required float HealthCoefficient { get; init; }
    public required float StrengthCoefficient { get; init; }
    public required float DexterityCoefficient { get; init; }
    public required float IntelligenceCoefficient { get; init; }
    public required float MoveSpeedCoefficient { get; init; }
    public required float AttackSpeedCoefficient { get; init; }
    public required List<string> DefaultCommand { get; init; }
    public required int CharacterId { get; init; }
    
    public Character? Character { get; init; }
    public IEnumerable<CustomCommand>? CustomCommands { get; init; }
}