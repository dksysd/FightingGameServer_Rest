using FightingGameServer_Rest.Dtos.MatchRecord;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IMatchRecordInfoService
{
    Task<IEnumerable<MatchRecordDto>> GetMatchRecordInfos(GetMatchRecordInfoRequestDto request);
}