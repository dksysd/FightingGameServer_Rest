using FightingGameServer_Rest.Domains.Player.Dtos;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IPlayerInfoService
{
    Task<PlayerDto> CreatePlayer(CreatePlayerRequestDto request, int userId);
    Task<PlayerDto> GetPlayerInfo(GetPlayerInfoRequestDto request);
}