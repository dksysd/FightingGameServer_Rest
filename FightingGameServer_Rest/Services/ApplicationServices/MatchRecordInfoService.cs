using FightingGameServer_Rest.Dtos.MatchRecord;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices;

namespace FightingGameServer_Rest.Services.ApplicationServices;

public class MatchRecordInfoService(MatchRecordService matchRecordService, IPlayerInfoService playerInfoService)
    : IMatchRecordInfoService
{
    public async Task<IEnumerable<MatchRecordDto>> GetMatchRecordInfos(GetMatchRecordInfoRequestDto request)
    {
        List<MatchRecord> matchRecords = await matchRecordService.GetAllMatchRecordsByPlayerId(request.PlayerId);
        return matchRecords.Select(matchRecord => matchRecord.ToDto());
    }
}