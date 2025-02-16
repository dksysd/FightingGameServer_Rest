using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.DataServices;

public class PlayerService(IPlayerRepository playerRepository) : IPlayerService
{
    public async Task<Player> CreatePlayer(Player player)
    {
        List<Player> players = await playerRepository.GetByUserId(player.UserId);
        if (players.Count != 0) throw new InvalidOperationException("User can only have one player.");
        Player? existingPlayer = await playerRepository.GetByName(player.Name);
        if (existingPlayer is not null) throw new InvalidOperationException("Player name already exists.");
        return await playerRepository.CreateAsync(player) ?? throw new InvalidOperationException("Create player failed.");
    }

    public async Task<Player> GetPlayerById(int id)
    {
        return await playerRepository.GetByIdAsync(id) ?? throw new InvalidOperationException("Player not found.");
    }

    public async Task<Player> GetPlayerByName(string name)
    {
        return await playerRepository.GetByName(name) ?? throw new InvalidOperationException("Player not found.");
    }

    public async Task<List<Player>> GetPlayerByUserId(int userId)
    {
        List<Player> players = await playerRepository.GetByUserId(userId);
        if (players.Count == 0) throw new InvalidOperationException("Player not found.");
        return players;
    }

    public async Task<Player> UpdatePlayer(Player player)
    {
        await GetPlayerById(player.Id);
        return await playerRepository.UpdateAsync(player.Id, player);
    }
}