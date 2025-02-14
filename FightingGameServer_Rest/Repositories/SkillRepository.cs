using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class SkillRepository(GameDbContext context) : Repository<Skill>(context), ISkillRepository
{
    public async Task<Skill?> GetByName(string name)
    {
        return await Context.Skills.FirstOrDefaultAsync(skill => skill.Name.Equals(name));
    }

    public async Task<IEnumerable<Skill?>> GetByCharacterId(int characterId)
    {
        return await Context.Skills.Where(skill => skill.CharacterId == characterId).ToListAsync();
    }
}