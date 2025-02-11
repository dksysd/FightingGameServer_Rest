using FightingGameServer_Rest.Dtos.Player;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IPlayerInfoService
{
    Task<CreatePlayerResponseDto> CreatePlayer(CreatePlayerRequestDto request, int userId);
    Task<GetPlayerInfoResponseDto> GetPlayerInfo(GetPlayerInfoRequestDto request);
}