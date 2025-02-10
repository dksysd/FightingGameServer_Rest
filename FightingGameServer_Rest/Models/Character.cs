namespace FightingGameServer_Rest.Models;

public class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Health { get; set; }
    public required int Strength { get; set; }
    public required int Dexterity { get; set; }
    public required int Intelligence { get; set; }
    public required float MoveSpeed { get; set; }
    public required float AttackSpeed { get; set; }
    
    public virtual IEnumerable<Skill>? Skills { get; set; }
    public virtual IEnumerable<CustomCommand>? CustomCommands { get; set; }
    public virtual IEnumerable<MatchRecord>? WonMatchRecords { get; set; }
    public virtual IEnumerable<MatchRecord>? LostMatchRecords { get; set; }
}