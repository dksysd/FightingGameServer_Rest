using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface ICustomCommandRepository
{
    Task<IEnumerable<CustomCommand?>> GetByPlayerId(int playerId);
    Task<CustomCommand> Create(CustomCommand customCommand);
    Task<CustomCommand> Update(CustomCommand customCommand);
    Task<CustomCommand> Delete(CustomCommand customCommand);
}