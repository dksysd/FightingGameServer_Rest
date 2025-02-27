using FightingGameServer_Rest.Domains.Character.Dtos;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface ICharacterInfoService
{
    Task<IEnumerable<CharacterDto>> GetAllCharacterInfos();
}