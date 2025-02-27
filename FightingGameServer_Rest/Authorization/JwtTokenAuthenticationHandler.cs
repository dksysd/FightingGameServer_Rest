using System.Diagnostics.CodeAnalysis;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace FightingGameServer_Rest.Authorization;

[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class JwtTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    JwtTokenExtractor jwtTokenExtractor) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
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
        SecurityToken validatedToken;
        
        try
        {
            validatedToken = jwtTokenExtractor.ExtractToken(accessToken);
        }
        catch (SecurityTokenExpiredException expiredException)
        {
            return Task.FromResult(AuthenticateResult.Fail(expiredException.Message));
        }
        

        JwtSecurityToken jwtSecurityToken = (JwtSecurityToken)validatedToken;
        Claim? userIdClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "nameid");
        if (userIdClaim is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("No user id claim"));
        }
        Claim? roleClaim = jwtSecurityToken.Claims.First(claim => claim.Type == "role");
        if (roleClaim is null)
        {
            return Task.FromResult(AuthenticateResult.Fail("No role claim"));
        }
        Claim? playerIdClaim = jwtSecurityToken.Claims.FirstOrDefault(claim => claim.Type == "playerId");

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, userIdClaim.Value),
            new (ClaimTypes.Role, roleClaim.Value),
        ];
        if (playerIdClaim is not null)
        {
            claims.Add(new Claim("playerId", playerIdClaim.Value));
        }
        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}