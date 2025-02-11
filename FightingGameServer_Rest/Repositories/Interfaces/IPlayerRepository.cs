using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface IPlayerRepository
{
    Task<Player?> GetById(int id);
    Task<Player?> GetByName(string name);
    Task<List<Player>> GetByUserId(int userId);
    Task<Player> Create(Player player);
    Task<Player> Update(Player player);
    Task<Player> Delete(Player player);
}