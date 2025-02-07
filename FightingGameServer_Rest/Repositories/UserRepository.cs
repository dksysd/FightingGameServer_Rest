using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace FightingGameServer_Rest.Repository;

public class UserRepository(GameDbContext context) : IUserRepository
{
    public async Task<User?> GetById(int id)
    {
        return await context.Users.FindAsync(id);
    }

    public async Task<User?> GetByLoginId(string loginId)
    {
        return await context.Users.FirstOrDefaultAsync(user => user.LoginId.Equals(loginId));
    }

    public async Task<User> Create(User user)
    {
        EntityEntry<User> createdUser = await context.Users.AddAsync(user);
        await context.SaveChangesAsync();
        return createdUser.Entity;
    }

    public async Task<User> Update(User user)
    {
        EntityEntry<User> updatedUser = context.Users.Update(user);
        await context.SaveChangesAsync();
        return updatedUser.Entity;
    }

    public async Task<User> Delete(User user)
    {
        EntityEntry<User> deletedUser = context.Users.Remove(user);
        await context.SaveChangesAsync();
        return deletedUser.Entity;
    }

    public async Task<User> DeleteById(int id)
    {
        User? user = await GetById(id);
        return await Delete(user ?? throw new InvalidOperationException("User not found"));
    }
}