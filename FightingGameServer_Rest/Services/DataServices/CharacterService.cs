using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Services.DataServices;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class CharacterService(ICharacterRepository characterRepository) : ICharacterService
{
    public async Task<IEnumerable<Character>> GetAllCharacters()
    {
        return await characterRepository.GetAllAsync(query => query.Include(character => character.Skills));
    }
}