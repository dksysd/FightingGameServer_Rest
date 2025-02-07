using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Dtos.Auth;

public class RefreshRequestDto
{
    [Required]
    public required string RefreshToken { get; set; }
}