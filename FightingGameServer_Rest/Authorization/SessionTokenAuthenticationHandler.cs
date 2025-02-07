using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;

namespace FightingGameServer_Rest.Authorization;

public class SessionTokenAuthenticationHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    IMemoryCache memoryCache) : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue("session-token", out StringValues tokenValues))
        {
            return Task.FromResult(AuthenticateResult.Fail("Session token is missing."));
        }

        string? sessionToken = tokenValues.FirstOrDefault();
        if (string.IsNullOrEmpty(sessionToken))
        {
            return Task.FromResult(AuthenticateResult.Fail("Session token is empty."));
        }

        if (!memoryCache.TryGetValue(sessionToken, out int userId))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid or expired session token."));
        }

        Claim[] claims =
        [
            new(ClaimTypes.NameIdentifier, userId.ToString())
        ];
        ClaimsIdentity identity = new(claims, Scheme.Name);
        ClaimsPrincipal principal = new(identity);
        AuthenticationTicket ticket = new(principal, Scheme.Name);

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}