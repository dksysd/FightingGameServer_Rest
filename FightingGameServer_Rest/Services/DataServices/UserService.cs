using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using FightingGameServer_Rest.Services.DataServices.Interfaces;

namespace FightingGameServer_Rest.Services.DataServices;

public class UserService(IUserRepository userRepository) : IUserService
{
    public async Task<User> CreateUser(User user)
    {
        User? existUser = await userRepository.GetByLoginId(user.LoginId);
        if (existUser != null) throw new InvalidOperationException("Login id is already used.");
        return await userRepository.Create(user);
    }

    public async Task<User> GetUserById(int userId)
    {
        return await userRepository.GetById(userId) ?? throw new InvalidOperationException("User not found.");
    }

    public async Task<User> GetUserByLoginId(string loginId)
    {
        return await userRepository.GetByLoginId(loginId) ?? throw new InvalidOperationException("User not found.");
    }


    public async Task<User> UpdateUser(User user)
    {
        await GetUserByLoginId(user.LoginId);
        return await userRepository.Update(user);
    }

    public async Task<User> DeleteUser(User user)
    {
        await GetUserByLoginId(user.LoginId);
        return await userRepository.Delete(user);
    }
}