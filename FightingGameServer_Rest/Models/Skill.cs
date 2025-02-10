namespace FightingGameServer_Rest.Models;

public class Skill
{
    public int Id { get; set; }
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
    public required string DefaultCommand { get; set; }
    public required int CharacterId { get; set; }
    
    public virtual Character? Character { get; set; }
    public virtual IEnumerable<CustomCommand>? CustomCommands { get; set; }
}