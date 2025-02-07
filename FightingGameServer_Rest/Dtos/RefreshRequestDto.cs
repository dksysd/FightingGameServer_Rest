using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Dtos;

public class RefreshRequestDto
{
    [Required]
    public required string RefreshToken { get; set; }
}