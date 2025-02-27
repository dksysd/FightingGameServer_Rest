namespace FightingGameServer_Rest.Domains.Player.Dtos;

public static class PlayerDtoExtension
{
    public static PlayerDto ToDto(this Models.Player player)
    {
        return new PlayerDto
        {
            Name = player.Name,
            ExperiencePoint = player.ExperiencePoint,
            MatchCount = player.MatchCount
        };
    }
}