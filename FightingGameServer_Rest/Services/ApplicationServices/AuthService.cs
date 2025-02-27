using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using FightingGameServer_Rest.Domains.Auth.Dtos;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Services.ApplicationServices.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.IdentityModel.Tokens;

namespace FightingGameServer_Rest.Services.ApplicationServices;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
public class AuthService(
    IUserService userService,
    IConfiguration configuration,
    IMemoryCache memoryCache) : IAuthService
{
    private readonly byte[] _secretKeyBytes = Encoding.UTF8.GetBytes(
        configuration.GetValue<string>("JwtSettings:SecretKey") ??
        throw new InvalidOperationException("Jwt secret key is null"));

    private readonly TimeSpan _accessTokenExpirationMinutes =
        TimeSpan.FromMinutes(configuration.GetValue<int>("JwtSettings:AccessTokenExpirationMinutes"));

    private readonly TimeSpan _refreshTokenExpirationMinutes =
        TimeSpan.FromMinutes(configuration.GetValue<int>("JwtSettings:RefreshTokenExpirationMinutes"));

    private readonly TimeSpan _webSocketTokenExpirationMinutes =
        TimeSpan.FromMinutes(configuration.GetValue<int>("JwtSettings:WebSocketTokenExpirationMinutes"));

    public async Task Register(RegisterRequestDto request)
    {
        byte[] saltBytes = GenerateSalt();
        string salt = Convert.ToBase64String(saltBytes);

        byte[] passwordHash = HashPassword(request.LoginPassword, saltBytes);
        string password = Convert.ToBase64String(passwordHash);

        User newUser = new()
        {
            LoginId = request.LoginId,
            LoginPassword = password,
            Salt = salt,
            Role = User.RoleType.User
        };

        await userService.CreateUser(newUser);
    }

    public async Task<LoginResponseDto> Login(LoginRequestDto request)
    {
        User user = await userService.GetUserByLoginId(request.LoginId);

        if (!VerifyPassword(request.LoginPassword, user.Salt, user.LoginPassword))
        {
            throw new InvalidOperationException("Invalid login password");
        }

        string accessToken = GenerateAccessToken(user.Id.ToString(), user.Role.ToString());
        string refreshToken = GenerateRefreshToken();

        MemoryCacheEntryOptions cacheEntryOptions = new();
        cacheEntryOptions.SetAbsoluteExpiration(_refreshTokenExpirationMinutes);
        memoryCache.Set(refreshToken, new Memory
        {
            UserId = user.Id,
            Role = user.Role
        }, cacheEntryOptions);

        return new LoginResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken,
            Role = user.Role.ToString()
        };
    }

    public void Logout(LogoutRequestDto request, int userId)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new InvalidOperationException("Refresh token is empty");
        }

        if (!memoryCache.TryGetValue(request.RefreshToken, out Memory? memory) || memory is null)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        if (memory.UserId != userId)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        memoryCache.Remove(request.RefreshToken);
    }

    public RefreshResponseDto Refresh(RefreshRequestDto request, int userId)
    {
        if (string.IsNullOrEmpty(request.RefreshToken))
        {
            throw new InvalidOperationException("Access token is empty");
        }

        if (!memoryCache.TryGetValue(request.RefreshToken, out Memory? memory) || memory is null)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        if (memory.UserId != userId)
        {
            throw new InvalidOperationException("Invalid refresh token");
        }

        memoryCache.Remove(request.RefreshToken);

        string refreshToken = GenerateRefreshToken();
        MemoryCacheEntryOptions cacheEntryOptions = new();
        cacheEntryOptions.SetAbsoluteExpiration(_refreshTokenExpirationMinutes);
        memoryCache.Set(refreshToken, memory, cacheEntryOptions);

        string accessToken = GenerateAccessToken(memory.UserId.ToString(), memory.Role.ToString());

        return new RefreshResponseDto
        {
            AccessToken = accessToken,
            RefreshToken = refreshToken
        };
    }

    public WebSocketTokenResponseDto GetWebSocketToken(string playerId)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescription = new()
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, playerId)
            ]),
            Expires = DateTime.UtcNow.Add(_webSocketTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKeyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken? token = tokenHandler.CreateToken(tokenDescription);
        return new WebSocketTokenResponseDto
        {
            WebSocketToken = tokenHandler.WriteToken(token)
        };
    }

    private static byte[] GenerateSalt(int saltSize = 64)
    {
        using RandomNumberGenerator randomGenerator = RandomNumberGenerator.Create();
        byte[] salt = new byte[saltSize];
        randomGenerator.GetBytes(salt);
        return salt;
    }

    private static byte[] HashPassword(string password, byte[] salt)
    {
        Argon2id argon2 = new(Encoding.UTF8.GetBytes(password));
        argon2.Salt = salt;
        argon2.DegreeOfParallelism = 4;
        argon2.MemorySize = 65536;
        argon2.Iterations = 3;

        return argon2.GetBytes(128);
    }

    private static bool VerifyPassword(string password, string salt, string correctHash)
    {
        byte[] newHash;
        try
        {
            Argon2id argon2 = new(Encoding.UTF8.GetBytes(password));
            argon2.Salt = Convert.FromBase64String(salt);
            argon2.DegreeOfParallelism = 4;
            argon2.MemorySize = 65536;
            argon2.Iterations = 3;
            newHash = argon2.GetBytes(128);
        }
        catch (Exception)
        {
            return false;
        }

        return CompareByteArrays(newHash, Convert.FromBase64String(correctHash));
    }

    private static bool CompareByteArrays(byte[] array1, byte[] array2)
    {
        return array1.SequenceEqual(array2);
    }

    private string GenerateAccessToken(string userId, string role)
    {
        JwtSecurityTokenHandler tokenHandler = new();
        SecurityTokenDescriptor tokenDescription = new()
        {
            Subject = new ClaimsIdentity([
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Role, role)
            ]),
            Expires = DateTime.UtcNow.Add(_accessTokenExpirationMinutes),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(_secretKeyBytes),
                SecurityAlgorithms.HmacSha256Signature)
        };
        SecurityToken? token = tokenHandler.CreateToken(tokenDescription);
        return tokenHandler.WriteToken(token);
    }

    private static string GenerateRefreshToken()
    {
        return Guid.NewGuid().ToString();
    }


    private class Memory
    {
        public int UserId { get; init; }
        public User.RoleType Role { get; init; }
    }
}