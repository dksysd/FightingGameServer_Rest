using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Authorization;

public static class RoleHierarchy
{
    public static bool IsInRole(string userRole, string requiredRole)
    {
        if (!Enum.TryParse(userRole, out User.RoleType userRoleType) ||
            !Enum.TryParse(requiredRole, out User.RoleType requiredRoleType))
        {
            throw new ArgumentException("Invalid role type");
        }
        
        return IsInRole(userRoleType, requiredRoleType);
    }

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