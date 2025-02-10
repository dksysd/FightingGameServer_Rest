using FightingGameServer_Rest.Models;
using FightingGameServer_Rest.Models.Configurations;
using Microsoft.EntityFrameworkCore;

namespace FightingGameServer_Rest.Data;

public class GameDbContext(DbContextOptions options) : DbContext(options)
{
    public DbSet<User> Users { get; set; }
    public DbSet<Player> Players { get; set; }
    public DbSet<Character> Characters { get; set; }
    public DbSet<Skill> Skills { get; set; }
    public DbSet<CustomCommand> CustomCommands { get; set; }
    public DbSet<MatchRecord> MatchRecords { get; set; }

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