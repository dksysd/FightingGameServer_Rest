using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.Auth.Dtos;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class WebSocketTokenResponseDto
{
    public required string WebSocketToken { get; set; }
}