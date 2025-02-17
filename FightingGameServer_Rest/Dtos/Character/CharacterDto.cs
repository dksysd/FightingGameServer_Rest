using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Dtos.Skills;

namespace FightingGameServer_Rest.Dtos.Character;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class CharacterDto
{
    public required string Name { get; set; }
    public required int Health { get; set; }
    public required int Strength { get; set; }
    public required int Dexterity { get; set; }
    public required int Intelligence { get; set; }
    public required float MoveSpeed { get; set; }
    public required float AttackSpeed { get; set; }
    
    public required IEnumerable<SkillDto> Skills { get; set; }
}