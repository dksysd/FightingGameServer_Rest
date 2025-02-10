using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface ICharacterRepository
{
    Task<IEnumerable<Character?>> GetAll();
    Task<Character?> GetById(int id);
    Task<Character?> GetByName(string name);
}