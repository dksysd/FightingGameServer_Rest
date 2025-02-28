using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.Auth.Dtos;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class LoginPlayerRequestDto
{
    public required string PlayerName { get; init; }
    public required string RefreshToken {get; init;}
}