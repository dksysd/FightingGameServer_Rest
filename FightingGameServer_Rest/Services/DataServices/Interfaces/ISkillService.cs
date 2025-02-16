using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Services.DataServices.Interfaces;

public interface ISkillService
{
    Task<IEnumerable<Skill>> GetAllSkills();
}