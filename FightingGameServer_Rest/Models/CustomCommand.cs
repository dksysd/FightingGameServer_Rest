namespace FightingGameServer_Rest.Models;

public class CustomCommand
{
    public int Id { get; set; }
    public required string Command { get; set; }
    public required int PlayerId { get; set; }
    public required int CharacterId { get; set; }
    public required int SkillId { get; set; }

    public virtual Player? Player { get; set; }
    public virtual Character? Character { get; set; }
    public virtual Skill? Skill { get; set; }
}