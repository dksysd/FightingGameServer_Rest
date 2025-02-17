namespace FightingGameServer_Rest.Models;

public sealed class Character
{
    public int Id { get; set; }
    public required string Name { get; set; }
    public required int Health { get; set; }
    public required int Strength { get; set; }
    public required int Dexterity { get; set; }
    public required int Intelligence { get; set; }
    public required float MoveSpeed { get; set; }
    public required float AttackSpeed { get; set; }
    
    public IEnumerable<Skill>? Skills { get; set; }
    public IEnumerable<CustomCommand>? CustomCommands { get; set; }
    public IEnumerable<MatchRecord>? WonMatchRecords { get; set; }
    public IEnumerable<MatchRecord>? LostMatchRecords { get; set; }
}