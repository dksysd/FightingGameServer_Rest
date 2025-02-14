using FightingGameServer_Rest.Dtos.Character;
using FightingGameServer_Rest.Dtos.Players;

namespace FightingGameServer_Rest.Dtos.MatchRecord;

public class GetMatchRecordInfoResponseDto
{
    public required List<MatchRecordInfoDto> matchRecordInfos { get; set; }

    public class MatchRecordInfoDto
    {
        public required DateTime StartedAt { get; set; }
        public required DateTime EndedAt { get; set; }
        public required PlayerDto WinnerPlayerDto { get; set; }
        public required CharacterDto WinnerPlayerCharacter { get; set; }
        public required PlayerDto LoserPlayerCharacterDto { get; set; }
        public required CharacterDto LoserPlayerCharacter { get; set; }
    }
}