using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface IUserRepository : IRepository<User>
{
    Task<User?> GetByLoginIdAsync(string loginId);
}