using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class CharacterRepository(GameDbContext context) : ICharacterRepository
{
    public async Task<IEnumerable<Character?>> GetAll()
    {
        return await context.Characters.ToListAsync();
    }

    public async Task<Character?> GetById(int id)
    {
        return await context.Characters.FindAsync(id);
    }

    public async Task<Character?> GetByName(string name)
    {
        return await context.Characters.FirstOrDefaultAsync(character => character.Name.Equals(name));
    }
}