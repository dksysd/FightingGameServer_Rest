using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Data;
using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Repositories;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
[SuppressMessage("ReSharper", "HeapView.ClosureAllocation")]
public class UserRepository(GameDbContext context) : Repository<User>(context), IUserRepository
{
    public async Task<User?> GetByLoginIdAsync(string loginId)
    {
        return await Context.Users.FirstOrDefaultAsync(user => user.LoginId.Equals(loginId));
    }
}