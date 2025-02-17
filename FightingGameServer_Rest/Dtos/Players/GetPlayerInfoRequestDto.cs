using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Dtos.Players;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GetPlayerInfoRequestDto
{
    [Required]
    public required string PlayerName { get; init; }
}