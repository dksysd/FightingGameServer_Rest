using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Dtos.Players;

public class CreatePlayerRequestDto
{
    [Required]
    [StringLength(20, MinimumLength = 1, ErrorMessage = "Username must be between 1 and 20 characters.")]
    public required string PlayerName { get; set; }
}