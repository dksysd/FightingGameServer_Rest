namespace FightingGameServer_Rest.Dtos;

public class LoginResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}