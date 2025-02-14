using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Dtos.Skills;

public static class SkillDtoExtension
{
    public static SkillDto ToDto(this Skill skill)
    {
        return new SkillDto
        {
            Name = skill.Name,
            Description = skill.Description,
            IsPassive = skill.IsPassive,
            CoolTime = skill.CoolTime,
            Range = skill.Range,
            HealthCoefficient = skill.HealthCoefficient,
            StrengthCoefficient = skill.StrengthCoefficient,
            DexterityCoefficient = skill.DexterityCoefficient,
            IntelligenceCoefficient = skill.IntelligenceCoefficient,
            MoveSpeedCoefficient = skill.MoveSpeedCoefficient,
            AttackSpeedCoefficient = skill.AttackSpeedCoefficient,
            DefaultCommand = skill.DefaultCommand
        };
    }
}