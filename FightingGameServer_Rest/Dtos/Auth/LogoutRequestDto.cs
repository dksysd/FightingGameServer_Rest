using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Dtos.Auth;

public class LogoutRequestDto
{
    [Required]
    public required string RefreshToken { get; set; }
}