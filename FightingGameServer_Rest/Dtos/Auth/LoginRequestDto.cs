namespace FightingGameServer_Rest.Dtos.Auth;

public class LoginRequestDto
{
    public required string LoginId { get; set; }
    public required string LoginPassword { get; set; }
}