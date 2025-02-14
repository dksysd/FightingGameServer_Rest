using FightingGameServer_Rest.Dtos.Players;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IPlayerInfoService
{
    Task<PlayerDto> CreatePlayer(CreatePlayerRequestDto request, int userId);
    Task<PlayerDto> GetPlayerInfo(GetPlayerInfoRequestDto request);
}