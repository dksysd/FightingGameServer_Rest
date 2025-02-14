using FightingGameServer_Rest.Dtos.Players;
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

    public async Task<PlayerDto> CreatePlayer(CreatePlayerRequestDto request, int userId)
    {
        Player player = await playerService.CreatePlayer(new Player
        {
            Name = request.PlayerName,
            ExperiencePoint = 0,
            MatchCount = 0,
            UserId = userId
        });
        return player.ToDto();
    }

    public async Task<PlayerDto> GetPlayerInfo(GetPlayerInfoRequestDto request)
    {
        Player player;
        try
        {
            player = await playerService.GetPlayerByName(request.PlayerName);
        }
        catch (InvalidOperationException invalidOperationException)
        {
            Console.WriteLine(invalidOperationException.StackTrace);
            player = _deletedPlayer;
        }

        return player.ToDto();
    }
}