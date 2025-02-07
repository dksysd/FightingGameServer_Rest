namespace FightingGameServer_Rest.Dtos;

public class RefreshResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}