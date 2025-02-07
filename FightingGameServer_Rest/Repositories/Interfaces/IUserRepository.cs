using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Repositories.Interfaces;

public interface IUserRepository
{
    Task<User?> GetById(int id);
    Task<User?> GetByLoginId(string loginId);
    Task<User> Create(User user);
    Task<User> Update(User user);
    Task<User> Delete(User user);
    Task<User> DeleteById(int id);
}