using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Dtos.Auth;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RefreshResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}