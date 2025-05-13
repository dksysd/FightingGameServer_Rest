namespace FightingGameServer_Rest.Models;

public sealed class Character
{
    public int Id { get; init; }
    public required string Name { get; init; }
    public required string Description { get; init; }
    public required int Health { get; init; }
    public required int Strength { get; init; }
    public required int Dexterity { get; init; }
    public required int Intelligence { get; init; }
    public required float MoveSpeed { get; init; }
    public required float AttackSpeed { get; init; }

    public IEnumerable<Skill>? Skills { get; init; }
    public IEnumerable<CustomCommand>? CustomCommands { get; init; }
    public IEnumerable<MatchRecord>? WonMatchRecords { get; init; }
    public IEnumerable<MatchRecord>? LostMatchRecords { get; init; }
}