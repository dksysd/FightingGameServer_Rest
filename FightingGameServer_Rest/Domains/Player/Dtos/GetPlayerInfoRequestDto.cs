using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.Player.Dtos;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GetPlayerInfoRequestDto
{
    [Required]
    public required string PlayerName { get; init; }
}