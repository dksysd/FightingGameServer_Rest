using FightingGameServer_Rest.Dtos.Player;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.Interfaces;

namespace FightingGameServer_Rest.Services;

public class PlayerService(IPlayerRepository playerRepository) : IPlayerService
{
    private readonly Player _deletedPlayer = new()
    {
        Name = "Deleted Player",
        ExperiencePoint = 0,
        MatchCount = 0,
        UserId = -1
    };


    public async Task<CreatePlayerResponseDto> CreatePlayer(CreatePlayerRequestDto createPlayerRequestDto, int userId)
    {
        IEnumerable<Player> players = await playerRepository.GetByUserId(userId);
        if (players.Any())
        {
            throw new InvalidOperationException("User can only have one player");
        }

        Player? player = await playerRepository.GetByName(createPlayerRequestDto.PlayerName);
        if (player is not null)
        {
            throw new InvalidOperationException("Player name already exists");
        }

        Player newPlayer = new()
        {
            Name = createPlayerRequestDto.PlayerName,
            ExperiencePoint = 0,
            MatchCount = 0,
            UserId = userId
        };

        newPlayer = await playerRepository.Create(newPlayer);

        return new CreatePlayerResponseDto
        {
            Name = newPlayer.Name,
            ExperiencePoint = newPlayer.ExperiencePoint,
            MatchCount = newPlayer.MatchCount
        };
    }

    public async Task<GetPlayerInfoResponseDto> GetPlayerInfo(GetPlayerInfoRequestDto getPlayerInfoRequestDto)
    {
        Player player = await playerRepository.GetByName(getPlayerInfoRequestDto.PlayerName) ?? _deletedPlayer;
        return new GetPlayerInfoResponseDto
        {
            Name = player.Name,
            ExperiencePoint = player.ExperiencePoint,
            MatchCount = player.MatchCount
        };
    }
}