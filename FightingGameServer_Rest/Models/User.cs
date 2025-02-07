using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Models;

[SuppressMessage("ReSharper", "EntityFramework.ModelValidation.UnlimitedStringLength")]
public class User
{
    public int Id { get; set; }

    public required string LoginId { get; set; }
    public required string LoginPassword { get; set; }
    public required string Salt { get; set; }

    public virtual Player? Player { get; set; }
}