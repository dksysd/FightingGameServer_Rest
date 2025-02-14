using FightingGameServer_Rest.Dtos.Character;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface ICharacterInfoService
{
    Task<IEnumerable<CharacterDto>> GetAllCharacterInfos();
}