namespace FightingGameServer_Rest.Dtos.CustomCommand;

public class CustomCommandDto
{
    public required List<string> Command { get; init; }
    public required string CharacterName { get; init; }
    public required string SkillName { get; init; }
}