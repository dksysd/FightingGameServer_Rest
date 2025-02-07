using FightingGameServer_Rest.Dtos;

namespace FightingGameServer_Rest.Services.Interfaces;

public interface IAuthService
{
    Task Register(RegisterRequestDto registerRequestDto);
    Task<LoginResponseDto> Login(LoginRequestDto loginRequestDto);
    void Logout(LogoutRequestDto sessionToken);
    RefreshResponseDto Refresh(RefreshRequestDto sessionToken);
}