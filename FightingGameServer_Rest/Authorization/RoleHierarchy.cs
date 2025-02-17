using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Authorization;

public static class RoleHierarchy
{
    public static bool IsInRole(User.RoleType userRoleType, User.RoleType requiredRoleType)
    {
        if (userRoleType == requiredRoleType) return true;

        return requiredRoleType switch
        {
            User.RoleType.Admin => false,
            User.RoleType.User => userRoleType == User.RoleType.Admin,
            _ => throw new ArgumentOutOfRangeException($"Invalid role type {requiredRoleType}")
        };
    }
}