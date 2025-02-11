using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Services.DataServices.Interfaces;

public interface IPlayerService
{
    Task<Player> CreatePlayer(Player player);
    Task<Player> GetPlayerById(int id);
    Task<Player> GetPlayerByName(string name);
    Task<List<Player>> GetPlayerByUserId(int userId);
    Task<Player> UpdatePlayer(Player player);
}