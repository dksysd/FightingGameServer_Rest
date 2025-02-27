using FightingGameServer_Rest.Domains.MatchRecord.Dtos;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IMatchRecordInfoService
{
    Task<IEnumerable<MatchRecordDto>> GetMatchRecordInfos(GetMatchRecordInfoRequestDto request);
}