using FightingGameServer_Rest.Models;
using Microsoft.AspNetCore.Authorization;

namespace FightingGameServer_Rest.Authorization;

public class MinimumRoleRequirement(User.RoleType minimumRole) : IAuthorizationRequirement
{
    public User.RoleType MinimumRole { get; } = minimumRole;
}