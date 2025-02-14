using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Services.DataServices.Interfaces;

public interface IMatchRecordService
{
    Task<MatchRecord> CreateMatchRecord(MatchRecord record);
    Task<List<MatchRecord>> GetAllMatchRecordsByPlayerId(int userId);
    Task<MatchRecord> GetMatchRecordById(int matchRecordId);
}