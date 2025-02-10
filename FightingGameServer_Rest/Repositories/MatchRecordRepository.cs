using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class MatchRecordRepository(GameDbContext context) : IMatchRecordRepository
{
    public async Task<IEnumerable<MatchRecord?>> GetAll()
    {
        return await context.MatchRecords.ToListAsync();
    }

    public async Task<MatchRecord?> GetById(int id)
    {
        return await context.MatchRecords.FindAsync(id);
    }

    public async Task<IEnumerable<MatchRecord?>> GetByUserId(int userId)
    {
        return await context.MatchRecords
            .Where(matchRecord => matchRecord.WinnerPlayerId == userId || matchRecord.LoserPlayerId == userId)
            .ToListAsync();
    }

    public async Task<IEnumerable<MatchRecord?>> GetByTime(DateTime begin, DateTime end)
    {
        return await context.MatchRecords
            .Where(matchRecord => matchRecord.StartedAt >= begin && matchRecord.EndedAt <= end).ToListAsync();
    }
}