using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
public class PlayerRepository(GameDbContext context) : IPlayerRepository
{
    public async Task<Player?> GetById(int id)
    {
        return await context.Players.FindAsync(id);
    }

    public async Task<Player?> GetByName(string name)
    {
        return await context.Players.FirstOrDefaultAsync(player => player.Name == name);
    }

    public async Task<IEnumerable<Player>> GetByUserId(int userId)
    {
        return await context.Players.Where(player => player.UserId == userId).ToListAsync();
    }

    public async Task<Player> Create(Player player)
    {
        EntityEntry<Player> createdPlayer = await context.Players.AddAsync(player);
        await context.SaveChangesAsync();
        return createdPlayer.Entity;
    }

    public async Task<Player> Update(Player player)
    {
        EntityEntry<Player> updatedPlayer = context.Players.Update(player);
        await context.SaveChangesAsync();
        return updatedPlayer.Entity;
    }

    public async Task<Player> Delete(Player player)
    {
        EntityEntry<Player> deletedPlayer = context.Players.Remove(player);
        await context.SaveChangesAsync();
        return deletedPlayer.Entity;
    }
}