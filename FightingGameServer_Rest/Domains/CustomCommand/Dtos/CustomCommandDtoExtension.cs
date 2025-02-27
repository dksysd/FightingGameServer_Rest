using FightingGameServer_Rest.Exceptions;

namespace FightingGameServer_Rest.Domains.CustomCommand.Dtos;

public static class CustomCommandDtoExtension
{
    public static CustomCommandDto ToDto(this Models.CustomCommand customCommand)
    {
        if (customCommand.Character == null) throw new ConvertDtoException("CustomCommand.Character is null");
        if (customCommand.Skill == null) throw new ConvertDtoException("CustomCommand.Skill is null");
        
        return new CustomCommandDto
        {
            Command = customCommand.Command,
            CharacterName = customCommand.Character.Name,
            SkillName = customCommand.Skill.Name
        };
    }
}