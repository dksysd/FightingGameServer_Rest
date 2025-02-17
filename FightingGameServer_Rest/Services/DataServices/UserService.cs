using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.DataServices;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> CreateUser(User user)
    {
        User? existUser = await userRepository.GetByLoginIdAsync(user.LoginId);
        if (existUser != null) throw new InvalidOperationException("Login id is already used.");
        return await userRepository.CreateAsync(user) ?? throw new InvalidOperationException("Create user failed.");
    }

    public async Task<User> GetUserById(int userId)
    {
        return await userRepository.GetByIdAsync(userId) ?? throw new InvalidOperationException("User not found.");
    }

    public async Task<User> GetUserByLoginId(string loginId)
    {
        return await userRepository.GetByLoginIdAsync(loginId) ??
               throw new InvalidOperationException("User not found.");
    }

    public async Task<User> UpdateUser(User user)
    {
        return await userRepository.UpdateAsync(user.Id, user);
    }

    public async Task<User> DeleteUser(User user)
    {
        return await userRepository.DeleteAsync(user.Id);
    }
}