using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Dtos.Auth;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class LoginRequestDto
{
    public required string LoginId { get; init; }
    public required string LoginPassword { get; init; }
}