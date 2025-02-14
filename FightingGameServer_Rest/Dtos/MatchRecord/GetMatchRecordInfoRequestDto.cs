using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Dtos.MatchRecord;

public class GetMatchRecordInfoRequestDto
{
    [Required]
    public required int PlayerId { get; set; }
}