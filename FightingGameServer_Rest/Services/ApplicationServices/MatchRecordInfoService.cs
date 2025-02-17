using FightingGameServer_Rest.Dtos.MatchRecord;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.ApplicationServices;

public class MatchRecordInfoService(
    IMatchRecordService matchRecordService,
    IPlayerService playerService)
    : IMatchRecordInfoService
{
    public async Task<IEnumerable<MatchRecordDto>> GetMatchRecordInfos(GetMatchRecordInfoRequestDto request)
    {
        Player player = await playerService.GetPlayerByName(request.PlayerName);
        List<MatchRecord> matchRecords = await matchRecordService.GetAllMatchRecordsByPlayerId(player.Id);
        return matchRecords.Select(matchRecord => matchRecord.ToDto());
    }
}