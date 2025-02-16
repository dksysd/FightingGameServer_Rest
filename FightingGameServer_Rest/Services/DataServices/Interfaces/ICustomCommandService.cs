using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Services.DataServices.Interfaces;

public interface ICustomCommandService
{
    Task<CustomCommand> CreateCustomCommand(CustomCommand customCommand);
    Task<CustomCommand> GetCustomCommandsById(int id);
    Task<IEnumerable<CustomCommand>> GetCustomCommandsByPlayerId(int playerId);
    Task<CustomCommand> UpdateCustomCommand(CustomCommand customCommand);
    Task<CustomCommand> DeleteCustomCommand(CustomCommand customCommand);
}