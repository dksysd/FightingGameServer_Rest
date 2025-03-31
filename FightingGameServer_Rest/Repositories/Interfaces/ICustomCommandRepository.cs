using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface ICustomCommandRepository : IRepository<CustomCommand>
{
    Task<IEnumerable<CustomCommand>> GetByPlayerId(int playerId,
        Func<IQueryable<CustomCommand>, IQueryable<CustomCommand>>? includeFunc = null);

    Task<bool> DeleteAllByPlayerId(int playerId);
}