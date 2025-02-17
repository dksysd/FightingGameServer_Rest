namespace FightingGameServer_Rest.Models;

public sealed class CustomCommand
{
    public int Id { get; set; }
    public required List<string> Command { get; init; }
    public required int PlayerId { get; init; }
    public required int CharacterId { get; init; }
    public required int SkillId { get; init; }

    public Player? Player { get; init; }
    public Character? Character { get; init; }
    public Skill? Skill { get; init; }
}