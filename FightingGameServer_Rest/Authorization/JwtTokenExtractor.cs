using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace FightingGameServer_Rest.Authorization;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class JwtTokenExtractor(IConfiguration configuration)
{
    private readonly byte[] _secretKeyBytes = Encoding.UTF8.GetBytes(
        configuration.GetValue<string>("JwtSettings:SecretKey") ??
        throw new InvalidOperationException("Jwt secret key is missing."));

    public SecurityToken ExtractToken(string jwtToken)
    {
        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        jwtSecurityTokenHandler.ValidateToken(jwtToken, new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        }, out SecurityToken validatedToken);
        return validatedToken;
    }
}