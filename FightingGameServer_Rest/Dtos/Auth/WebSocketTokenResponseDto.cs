using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Dtos.Auth;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class WebSocketTokenResponseDto
{
    public required string WebSocketToken { get; set; }
}