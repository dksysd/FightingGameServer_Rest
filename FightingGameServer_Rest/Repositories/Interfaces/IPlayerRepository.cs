using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface IPlayerRepository : IRepository<Player>
{
    Task<Player?> GetByName(string name);
    Task<List<Player>> GetByUserId(int userId);
}