namespace FightingGameServer_Rest.Dtos.Skills;

public class SkillDto
{
    public required string Name { get; set; }
    public required string Description { get; set; }
    public required bool IsPassive { get; set; }
    public required int CoolTime { get; set; }
    public required int Range { get; set; }
    public required float HealthCoefficient { get; set; }
    public required float StrengthCoefficient { get; set; }
    public required float DexterityCoefficient { get; set; }
    public required float IntelligenceCoefficient { get; set; }
    public required float MoveSpeedCoefficient { get; set; }
    public required float AttackSpeedCoefficient { get; set; }
    public required List<string> DefaultCommand { get; set; }
}