namespace FightingGameServer_Rest.Domains.CustomCommand.Dtos;

public class CustomCommandDto
{
    public required List<string> Command { get; init; }
    public required string CharacterName { get; init; }
    public required string SkillName { get; init; }
}