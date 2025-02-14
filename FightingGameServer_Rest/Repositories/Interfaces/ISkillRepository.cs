using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface ISkillRepository : IRepository<Skill>
{
    Task<Skill?> GetByName(string name);
    Task<IEnumerable<Skill?>> GetByCharacterId(int characterId);
}