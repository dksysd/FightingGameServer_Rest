using System.Security.Cryptography;
using System.Text;
using FightingGameServer_Rest.Dtos;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repository.Interfaces;
using FightingGameServer_Rest.Services.Interfaces;
using Konscious.Security.Cryptography;
using Microsoft.Extensions.Caching.Memory;

namespace FightingGameServer_Rest.Services;

public class AuthService(
    IUserRepository userRepository,
    IConfiguration configuration,
    IMemoryCache memoryCache,
    ILogger<AuthService> logger) : IAuthService
{
    private readonly TimeSpan _sessionTimeout =
        TimeSpan.FromSeconds(configuration.GetValue<int>("SessionSettings:SessionExpirySeconds"));

    public async Task Register(RegisterRequestDto registerRequestDto)
    {
        User? existingUser = await userRepository.GetByLoginId(registerRequestDto.LoginId);
        if (existingUser is not null)
        {
            throw new InvalidOperationException("User already exists");
        }

        byte[] saltBytes = GenerateSalt();
        string salt = Convert.ToBase64String(saltBytes);

        byte[] passwordHash = HashPassword(registerRequestDto.LoginPassword, saltBytes);
        string password = Convert.ToBase64String(passwordHash);

        User newUser = new()
        {
            LoginId = registerRequestDto.LoginId,
            LoginPassword = password,
            Salt = salt,
        };

        await userRepository.Create(newUser);
    }
    
    public async Task<string> Login(LoginRequestDto loginRequestDto)
    {
        User? user = await userRepository.GetByLoginId(loginRequestDto.LoginId);
        if (user is null)
        {
            throw new InvalidOperationException("User does not exist");
        }

        if (!VerifyPassword(loginRequestDto.LoginPassword, user.Salt, user.LoginPassword))
        {
            throw new InvalidOperationException("Invalid login password");
        }

        string sessionToken = GenerateSessionToken();

        MemoryCacheEntryOptions cacheEntryOptions = new();
        cacheEntryOptions.SetSlidingExpiration(_sessionTimeout);
        memoryCache.Set(sessionToken, user.Id, cacheEntryOptions);
        
        return sessionToken;
    }

    public void Logout(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
        {
            throw new InvalidOperationException("Session token is empty");
        }
        
        memoryCache.Remove(sessionToken);
    }

    public void Heartbeat(string sessionToken)
    {
        if (string.IsNullOrEmpty(sessionToken))
        {
            throw new InvalidOperationException("Session token is empty");
        }

        if (!memoryCache.TryGetValue(sessionToken, out int _))
        {
            throw new InvalidOperationException("Session token expired or invalid");
        }
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

    private static string GenerateSessionToken(int tokenLength = 32)
    {
        return Guid.NewGuid().ToString();
    }
}