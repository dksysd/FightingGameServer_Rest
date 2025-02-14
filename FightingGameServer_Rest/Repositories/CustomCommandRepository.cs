using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class CustomCommandRepository(GameDbContext context)
    : Repository<CustomCommand>(context), ICustomCommandRepository
{
    public async Task<IEnumerable<CustomCommand?>> GetByPlayerId(int playerId)
    {
        return await Context.CustomCommands.Where(customCommand => customCommand.PlayerId == playerId).ToListAsync();
    }
}