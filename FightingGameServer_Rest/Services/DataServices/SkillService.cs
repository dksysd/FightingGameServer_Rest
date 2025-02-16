using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.DataServices;

public class SkillService (ISkillRepository skillRepository) : ISkillService
{
    public async Task<IEnumerable<Skill>> GetAllSkills()
    {
        return await skillRepository.GetAllAsync();
    }
}