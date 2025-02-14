using FightingGameServer_Rest.Dtos.Character;
using FightingGameServer_Rest.Dtos.Players;

namespace FightingGameServer_Rest.Dtos.MatchRecord;

public class MatchRecordDto
{
    public required DateTime StartedAt { get; set; }
    public required DateTime EndedAt { get; set; }

    public required PlayerDto WinnerPlayer { get; set; }
    public required CharacterDto WinnerCharacter { get; set; }
    public required PlayerDto LoserPlayer { get; set; }
    public required CharacterDto LoserCharacter { get; set; }
}