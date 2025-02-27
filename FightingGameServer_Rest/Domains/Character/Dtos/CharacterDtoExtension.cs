using FightingGameServer_Rest.Domains.Skill.Dtos;
using FightingGameServer_Rest.Exceptions;

namespace FightingGameServer_Rest.Domains.Character.Dtos;

public static class CharacterDtoExtension
{
    public static CharacterDto ToDto(this Models.Character character)
    {
        if (character.Skills is null) throw new ConvertDtoException("Skills cannot be null.");
        
        return new CharacterDto
        {
            Name = character.Name,
            Health = character.Health,
            Strength = character.Strength,
            Dexterity = character.Dexterity,
            Intelligence = character.Intelligence,
            MoveSpeed = character.MoveSpeed,
            AttackSpeed = character.AttackSpeed,
            Skills = character.Skills.Select(skill => skill.ToDto())
        };
    }
}