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
public class PlayerRepository(GameDbContext context) : Repository<Player>(context), IPlayerRepository
{
    public async Task<Player?> GetByName(string name)
    {
        return await Context.Players.FirstOrDefaultAsync(player => player.Name.Equals(name));
    }

    public async Task<List<Player>> GetByUserId(int userId)
    {
        return await Context.Players.Where(player => player.UserId == userId).ToListAsync();
    }
}