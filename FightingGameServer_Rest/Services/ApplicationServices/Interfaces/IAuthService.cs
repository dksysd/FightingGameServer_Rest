using FightingGameServer_Rest.Dtos.Auth;

namespace FightingGameServer_Rest.Services.ApplicationServices.Interfaces;

public interface IAuthService
{
    Task Register(RegisterRequestDto request);
    Task<LoginResponseDto> Login(LoginRequestDto request);
    void Logout(LogoutRequestDto request, int userId);
    RefreshResponseDto Refresh(RefreshRequestDto request, int userId);
}