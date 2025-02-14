using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Services.DataServices;

[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class MatchRecordService(IMatchRecordRepository matchRecordRepository) : IMatchRecordService
{
    public async Task<MatchRecord> CreateMatchRecord(MatchRecord record)
    {
        if (record.WinnerPlayerId is null) throw new InvalidOperationException("Winner player id is null");
        if (record.LoserPlayerId is null) throw new InvalidOperationException("Loser player id is null");
        
        if (record.StartedAt >= record.EndedAt)
            throw new InvalidOperationException("StartedAt must be less than EndedAt.");
        if (record.WinnerPlayerId == record.LoserPlayerId)
            throw new InvalidOperationException("Winner player id and loser player id must be different.");
        List<MatchRecord> matchRecordsAtTime = await matchRecordRepository.GetByTime(record.StartedAt, record.EndedAt,
            record.WinnerPlayerId.Value, record.LoserPlayerId.Value);
        if (matchRecordsAtTime.Count != 0)
            throw new InvalidOperationException("There is a match record at that point.");
        return await matchRecordRepository.CreateAsync(record) ??
               throw new InvalidOperationException("Create match record failed.");
    }

    public async Task<List<MatchRecord>> GetAllMatchRecordsByPlayerId(int userId)
    {
        List<MatchRecord> matchRecords = await matchRecordRepository.GetByUserId(userId,
            query => query.Include(record => record.WinnerPlayer).Include(record => record.LoserPlayer)
                .Include(record => record.WinnerPlayerCharacter).Include(record => record.LoserPlayerCharacter));
        if (matchRecords.Count == 0) throw new InvalidOperationException("No match records found.");
        return matchRecords;
    }

    public async Task<MatchRecord> GetMatchRecordById(int matchRecordId)
    {
        return await matchRecordRepository.GetByIdAsync(matchRecordId) ??
               throw new InvalidOperationException("No match record found.");
    }
}