using FightingGameServer_Rest.Dtos.Character;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.ApplicationServices;

public class CharacterInfoService (ICharacterService characterService) : ICharacterInfoService
{
    public async Task<IEnumerable<CharacterDto>> GetAllCharacterInfos()
    {
        IEnumerable<Character> characters = await characterService.GetAllCharacters();
        return characters.Select(character => character.ToDto());
    }
}