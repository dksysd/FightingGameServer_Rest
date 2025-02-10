using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class CustomCommandRepository(GameDbContext context) : ICustomCommandRepository
{
    public async Task<IEnumerable<CustomCommand?>> GetByPlayerId(int playerId)
    {
        return await context.CustomCommands.Where(customCommand => customCommand.PlayerId == playerId).ToListAsync();
    }

    public async Task<CustomCommand> Create(CustomCommand customCommand)
    {
        EntityEntry<CustomCommand> createdCustomCommand = await context.CustomCommands.AddAsync(customCommand);
        await context.SaveChangesAsync();
        return createdCustomCommand.Entity;
    }

    public async Task<CustomCommand> Update(CustomCommand customCommand)
    {
        EntityEntry<CustomCommand> updatedCustomCommand = context.CustomCommands.Update(customCommand);
        await context.SaveChangesAsync();
        return updatedCustomCommand.Entity;
    }

    public async Task<CustomCommand> Delete(CustomCommand customCommand)
    {
        EntityEntry<CustomCommand> deletedCustomCommand = context.CustomCommands.Remove(customCommand);
        await context.SaveChangesAsync();
        return deletedCustomCommand.Entity;
    }
}