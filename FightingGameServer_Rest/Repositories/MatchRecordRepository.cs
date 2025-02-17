using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class MatchRecordRepository(GameDbContext context) : Repository<MatchRecord>(context), IMatchRecordRepository
{
    public Task<List<MatchRecord>> GetByUserId(int userId,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null)
    {
        IQueryable<MatchRecord> query = Context.MatchRecords.Where(matchRecord =>
            matchRecord.WinnerPlayerId == userId || matchRecord.LoserPlayerId == userId);
        if (includeFunc is not null)
        {
            query = includeFunc(query);
        }

        return query.ToListAsync();
    }

    public Task<List<MatchRecord>> GetByTime(DateTime begin, DateTime end,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null)
    {
        IQueryable<MatchRecord> query = Context.MatchRecords
            .Where(matchRecord => matchRecord.StartedAt >= begin && matchRecord.EndedAt <= end);
        if (includeFunc is not null)
        {
            query = includeFunc(query);
        }

        return query.ToListAsync();
    }

    public Task<List<MatchRecord>> GetByTime(DateTime begin, DateTime end, int userId,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null)
    {
        IQueryable<MatchRecord> query = Context.MatchRecords
            .Where(matchRecord => matchRecord.StartedAt >= begin && matchRecord.EndedAt <= end).Where(matchRecord =>
                matchRecord.WinnerPlayerId == userId || matchRecord.LoserPlayerId == userId);
        if (includeFunc is not null)
        {
            query = includeFunc(query);
        }

        return query.ToListAsync();
    }

    public Task<List<MatchRecord>> GetByTime(DateTime begin, DateTime end, int userId1, int userId2,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null)
    {
        IQueryable<MatchRecord> query = Context.MatchRecords
            .Where(matchRecord => matchRecord.StartedAt >= begin && matchRecord.EndedAt <= end).Where(matchRecord =>
                matchRecord.WinnerPlayerId == userId1 ||
                matchRecord.WinnerPlayerId == userId2 ||
                matchRecord.LoserPlayerId == userId1 ||
                matchRecord.LoserPlayerId == userId2);
        if (includeFunc is not null)
        {
            query = includeFunc(query);
        }

        return query.ToListAsync();
    }
}