using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Services.DataServices;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.DelegateAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class CustomCommandService(ICustomCommandRepository customCommandRepository) : ICustomCommandService
{
    public async Task<CustomCommand> CreateCustomCommand(CustomCommand customCommand)
    {
        if (customCommand.Character is null) throw new InvalidOperationException("Character is required.");
        if (customCommand.Skill is null) throw new InvalidOperationException("Skill is required.");

        IEnumerable<CustomCommand> playerCustomCommands = await customCommandRepository.GetByPlayerId(
            customCommand.PlayerId,
            query => query.Include(c => c.Character).Include(c => c.Skill));
        CustomCommand? existCustomCommand = playerCustomCommands.FirstOrDefault(c =>
            c.Character!.Name == customCommand.Character.Name && c.Skill!.Name == customCommand.Skill.Name);
        if (existCustomCommand != null) throw new InvalidOperationException("Custom command is already exist.");
        return await customCommandRepository.CreateAsync(customCommand) ??
               throw new InvalidOperationException("Create custom command failed.");
    }

    public async Task<CustomCommand> GetCustomCommandsById(int id)
    {
        return await customCommandRepository.GetByIdAsync(id) ??
               throw new InvalidOperationException("Custom command not found.");
    }

    public async Task<IEnumerable<CustomCommand>> GetCustomCommandsByPlayerId(int playerId)
    {
        return await customCommandRepository.GetByPlayerId(playerId,
                   query => query.Include(customCommand => customCommand.Character)
                       .Include(customCommand => customCommand.Skill)) ??
               throw new InvalidOperationException("Custom commands not found.");
    }

    public async Task<CustomCommand> UpdateCustomCommand(CustomCommand customCommand)
    {
        return await customCommandRepository.UpdateAsync(customCommand.Id, customCommand);
    }

    public async Task<CustomCommand> DeleteCustomCommand(CustomCommand customCommand)
    {
        return await customCommandRepository.DeleteAsync(customCommand.Id, customCommand);
    }
}