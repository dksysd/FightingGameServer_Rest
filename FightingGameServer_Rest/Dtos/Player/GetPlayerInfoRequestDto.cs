using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Dtos.Player;

public class GetPlayerInfoRequestDto
{
    [Required]
    public required string PlayerName { get; set; }
}