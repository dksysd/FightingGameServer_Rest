using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Models;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class Player
{
    public int Id { get; set; }

    public required string Name { get; set; }
    public required int ExperiencePoint { get; set; }
    public required int MatchCount { get; set; }
    
    public required int UserId { get; set; }
    public User? User { get; set; }
}