using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.Auth.Dtos;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class RefreshRequestDto
{
    [Required]
    public required string RefreshToken { get; init; }
}