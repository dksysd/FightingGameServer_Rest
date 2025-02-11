using FightingGameServer_Rest.Dtos.Player;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.ApplicationServices;

public class PlayerInfoService(IPlayerService playerService) : IPlayerInfoService
{
    private readonly Player _deletedPlayer = new()
    {
        Name = "Deleted Player",
        ExperiencePoint = 0,
        MatchCount = 0,
        UserId = -1
    };

    public async Task<CreatePlayerResponseDto> CreatePlayer(CreatePlayerRequestDto request, int userId)
    {
        Player player = await playerService.CreatePlayer(new Player
        {
            Name = request.PlayerName,
            ExperiencePoint = 0,
            MatchCount = 0,
            UserId = userId
        });
        return new CreatePlayerResponseDto
        {
            Name = player.Name,
            ExperiencePoint = player.ExperiencePoint,
            MatchCount = player.MatchCount
        };
    }

    public async Task<GetPlayerInfoResponseDto> GetPlayerInfo(GetPlayerInfoRequestDto request)
    {
        Player player;
        try
        {
            player = await playerService.GetPlayerByName(request.PlayerName);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            player = _deletedPlayer;
        }
        
        return new GetPlayerInfoResponseDto
        {
            Name = player.Name,
            ExperiencePoint = player.ExperiencePoint,
            MatchCount = player.MatchCount
        };
    }
}