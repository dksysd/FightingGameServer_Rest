using System.Security.Claims;
using FightingGameServer_Rest.Models;
using Microsoft.AspNetCore.Authorization;

namespace FightingGameServer_Rest.Authorization;

public class MinimumRoleHandler : AuthorizationHandler<MinimumRoleRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumRoleRequirement requirement)
    {
        if (context.User.Identity is { IsAuthenticated: false }) return Task.CompletedTask;
        
        Claim? roleClaim = context.User.FindFirst(ClaimTypes.Role);
        if (roleClaim is null || string.IsNullOrEmpty(roleClaim.Value)) return Task.CompletedTask;
        
        if(!Enum.TryParse(roleClaim.Value, out User.RoleType userRoleType)) return Task.CompletedTask;
        User.RoleType requiredRole = requirement.MinimumRole;
        if (RoleHierarchy.IsInRole(userRoleType, requiredRole)) context.Succeed(requirement);
        
        return Task.CompletedTask;
    }
}