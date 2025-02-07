using FightingGameServer_Rest.Dtos;

namespace FightingGameServer_Rest.Services.Interfaces;

public interface IAuthService
{
    Task Register(RegisterRequestDto registerRequestDto);
    Task<string> Login(LoginRequestDto loginRequestDto);
    void Logout(string sessionToken);
    void Heartbeat(string sessionToken);
}