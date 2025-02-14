using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Dtos.Players;

public static class PlayerDtoExtension
{
    public static PlayerDto ToDto(this Player player)
    {
        return new PlayerDto
        {
            Name = player.Name,
            ExperiencePoint = player.ExperiencePoint,
            MatchCount = player.MatchCount
        };
    }
}