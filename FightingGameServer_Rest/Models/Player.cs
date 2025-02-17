using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Models;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class Player
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public required int ExperiencePoint { get; set; }
    public required int MatchCount { get; set; }
    public required int UserId { get; set; }

    public User? User { get; set; }
    public IEnumerable<CustomCommand>? CustomCommands { get; set; }
    public IEnumerable<MatchRecord>? WonMatchRecords { get; set; }
    public IEnumerable<MatchRecord>? LostMatchRecords { get; set; }
}