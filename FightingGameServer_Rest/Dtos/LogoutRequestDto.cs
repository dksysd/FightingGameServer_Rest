using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Dtos;

public class LogoutRequestDto
{
    [Required]
    public required string RefreshToken { get; set; }
}