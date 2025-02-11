using FightingGameServer_Rest.Models;

namespace FightingGameServer_Rest.Services.DataServices.Interfaces;

public interface IUserService
{
    Task<User> CreateUser(User user);
    Task<User> GetUserById(int userId);
    Task<User> GetUserByLoginId(string loginId);
    Task<User> UpdateUser(User user);
    Task<User> DeleteUser(User user);
}