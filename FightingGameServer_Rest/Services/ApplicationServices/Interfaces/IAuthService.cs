using FightingGameServer_Rest.Domains.Auth.Dtos;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IAuthService
{
    Task Register(RegisterRequestDto request);
    Task<LoginResponseDto> Login(LoginRequestDto request);
    void Logout(LogoutRequestDto request, int userId);
    RefreshResponseDto Refresh(RefreshRequestDto request, int userId);
    Task<LoginResponseDto> LoginPlayer(LoginPlayerRequestDto request, int userId);
    WebSocketTokenResponseDto GetWebSocketToken(string playerId);
}