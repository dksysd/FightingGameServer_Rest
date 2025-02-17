using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Models;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public sealed class User
{
    public enum RoleType
    {
        Admin,
        User
    }

    public int Id { get; init; }
    public required string LoginId { get; init; }
    public required string LoginPassword { get; init; }
    public required string Salt { get; init; }
    public required RoleType Role { get; init; }

    public Player? Player { get; init; }
}