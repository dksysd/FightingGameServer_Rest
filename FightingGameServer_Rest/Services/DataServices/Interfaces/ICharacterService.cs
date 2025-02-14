using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Services.DataServices.Interfaces;

public interface ICharacterService
{
    Task<IEnumerable<Character>> GetAllCharacters();
}