using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Models;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class Player
{
    public int Id { get; init; }

    public required string Name { get; init; }
    public required int ExperiencePoint { get; init; }
    public required int MatchCount { get; init; }
    public required int UserId { get; init; }

    public User? User { get; init; }
    public IEnumerable<CustomCommand>? CustomCommands { get; init; }
    public IEnumerable<MatchRecord>? WonMatchRecords { get; init; }
    public IEnumerable<MatchRecord>? LostMatchRecords { get; init; }
}