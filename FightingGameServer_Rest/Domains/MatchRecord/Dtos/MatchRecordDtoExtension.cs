using FightingGameServer_Rest.Domains.Character.Dtos;
using FightingGameServer_Rest.Domains.Player.Dtos;
using FightingGameServer_Rest.Exceptions;

namespace FightingGameServer_Rest.Domains.MatchRecord.Dtos;

public static class MatchRecordDtoExtension
{
    public static MatchRecordDto ToDto(this Models.MatchRecord matchRecord)
    {
        if (matchRecord.WinnerPlayer is null) throw new ConvertDtoException("MatchRecord.WinnerPlayer is null");
        if (matchRecord.WinnerPlayerCharacter is null)
            throw new ConvertDtoException("MatchRecord.WinnerPlayerCharacter is null");
        if (matchRecord.LoserPlayer is null) throw new ConvertDtoException("MatchRecord.LoserPlayer is null.");
        if (matchRecord.LoserPlayerCharacter is null)
            throw new ConvertDtoException("MatchRecord.LoserPlayerCharacter is null.");

        return new MatchRecordDto
        {
            StartedAt = matchRecord.StartedAt,
            EndedAt = matchRecord.EndedAt,
            WinnerPlayer = matchRecord.WinnerPlayer.ToDto(),
            WinnerCharacter = matchRecord.WinnerPlayerCharacter.ToDto(),
            LoserPlayer = matchRecord.LoserPlayer.ToDto(),
            LoserCharacter = matchRecord.LoserPlayerCharacter.ToDto()
        };
    }
}