namespace FightingGameServer_Rest.Dtos;

public class LoginRequestDto
{
    public required string LoginId { get; set; }
    public required string LoginPassword { get; set; }
}