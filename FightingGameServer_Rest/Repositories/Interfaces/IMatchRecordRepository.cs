using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface IMatchRecordRepository : IRepository<MatchRecord>
{
    Task<List<MatchRecord>> GetByUserId(int userId,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null);

    Task<List<MatchRecord>> GetByTime(DateTime begin, DateTime end,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null);

    Task<List<MatchRecord>> GetByTime(DateTime begin, DateTime end, int userId,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null);

    Task<List<MatchRecord>> GetByTime(DateTime begin, DateTime end, int userId1, int userId2,
        Func<IQueryable<MatchRecord>, IQueryable<MatchRecord>>? includeFunc = null);
}