using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Data;

public class GameDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new PlayerConfiguration());
        modelBuilder.ApplyConfiguration(new CharacterConfiguration());
        modelBuilder.ApplyConfiguration(new SkillConfiguration());
        modelBuilder.ApplyConfiguration(new CustomCommandConfiguration());
        modelBuilder.ApplyConfiguration(new MatchRecordConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}