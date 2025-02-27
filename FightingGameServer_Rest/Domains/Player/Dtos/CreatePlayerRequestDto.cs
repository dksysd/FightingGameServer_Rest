using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.Player.Dtos;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class CreatePlayerRequestDto
{
    [Required]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 20 characters.")]
    public required string PlayerName { get; init; }
}