using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface IMatchRecordRepository
{
    Task<IEnumerable<MatchRecord?>> GetAll();
    Task<MatchRecord?> GetById(int id);
    Task<IEnumerable<MatchRecord?>> GetByUserId(int userId);
    Task<IEnumerable<MatchRecord?>> GetByTime(DateTime begin, DateTime end);
}