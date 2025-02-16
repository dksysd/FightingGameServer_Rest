namespace FightingGameServer_Rest.Dtos.CustomCommand;

public class CustomCommandDto
{
    public required string Command { get; set; }
    public required string CharacterName { get; set; }
    public required string SkillName { get; set; }
}