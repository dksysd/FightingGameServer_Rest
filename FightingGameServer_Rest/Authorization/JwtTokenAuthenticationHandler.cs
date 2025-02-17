using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace FightingGameServer_Rest.Authorization;

public class JwtTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IConfiguration configuration) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    private readonly byte[] _secretKeyBytes = Encoding.UTF8.GetBytes(
        configuration.GetValue<string>("JwtSettings:SecretKey") ??
        throw new InvalidOperationException("Jwt secret key is missing."));

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("Authorization", out StringValues authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.Fail("Missing Authorization Header"));
        }

        string? accessToken = authorizationHeader.FirstOrDefault();
        if (string.IsNullOrEmpty(accessToken) || !accessToken.StartsWith("Bearer "))
        {
            return Task.FromResult(AuthenticateResult.Fail("Access token is missing"));
        }

        accessToken = accessToken["Bearer ".Length..];

        JwtSecurityTokenHandler jwtSecurityTokenHandler = new();
        SecurityToken validatedToken;
        try
        {
            jwtSecurityTokenHandler.ValidateToken(accessToken, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(_secretKeyBytes),
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = true,
                ClockSkew = TimeSpan.Zero
            }, out validatedToken);
        }
        catch (SecurityTokenExpiredException expiredException)
        {
            return Task.FromResult(AuthenticateResult.Fail(expiredException.Message));
        }
        

        JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)validatedToken;
        string userId =
            jwtSecurityToken.Claims.First(claim => claim.Type == "nameid").Value ??
            throw new InvalidOperationException("No user id claim");
        string role = jwtSecurityToken.Claims.First(claim => claim.Type == "role").Value ?? throw new InvalidOperationException("No role claim");

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, userId),
            new (ClaimTypes.Role, role)
        ];
        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}