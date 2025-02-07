using FightingGameServer_Rest.Dtos.Player;
using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Services.Interfaces;

public interface IPlayerService
{
    Task<CreatePlayerResponseDto> CreatePlayer(CreatePlayerRequestDto createPlayerRequestDto, int userId);
    Task<GetPlayerInfoResponseDto> GetPlayerInfo(GetPlayerInfoRequestDto getPlayerInfoRequestDto);
}