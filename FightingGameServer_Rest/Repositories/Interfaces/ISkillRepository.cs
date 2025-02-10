using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface ISkillRepository
{
    Task<IEnumerable<Skill?>> GetAll();
    Task<Skill?> GetById(int id);
    Task<Skill?> GetByName(string name);
    Task<IEnumerable<Skill?>> GetByCharacterId(int characterId);
}