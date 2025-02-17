namespace FightingGameServer_Rest.Models;

public sealed class CustomCommand
{
    public int Id { get; set; }
    public required List<string> Command { get; set; }
    public required int PlayerId { get; set; }
    public required int CharacterId { get; set; }
    public required int SkillId { get; set; }

    public Player? Player { get; set; }
    public Character? Character { get; set; }
    public Skill? Skill { get; set; }
}