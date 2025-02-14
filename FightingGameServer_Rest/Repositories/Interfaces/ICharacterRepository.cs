using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface ICharacterRepository : IRepository<Character>
{
    Task<Character?> GetByName(string name);
}