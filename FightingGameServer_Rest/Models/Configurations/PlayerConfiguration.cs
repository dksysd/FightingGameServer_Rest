using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FightingGameServer_Rest.Models.Configurations;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
public class PlayerConfiguration : IEntityTypeConfiguration<Player>
{
    public void Configure(EntityTypeBuilder<Player> builder)
    {
        builder.ToTable("player");
        builder.HasKey(player => player.Id);

        builder.Property(player => player.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(player => player.Name).HasColumnName("name").HasMaxLength(20).IsRequired();
        builder.HasIndex(player => player.Name).IsUnique();

        builder.Property(player => player.ExperiencePoint).HasColumnName("experience_point").HasDefaultValue(0)
            .IsRequired();

        builder.Property(player => player.MatchCount).HasColumnName("match_count").HasDefaultValue(0).IsRequired();

        builder.Property(player => player.UserId).HasColumnName("user_id").HasDefaultValue(-1).IsRequired();
        builder.HasOne(player => player.User).WithOne(user => user.Player);

        builder.HasMany(player => player.CustomCommands).WithOne(command => command.Player)
            .HasForeignKey(command => command.PlayerId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(player => player.WonMatchRecords).WithOne(matchRecord => matchRecord.WinnerPlayer)
            .HasForeignKey(matchRecord => matchRecord.WinnerPlayerId).OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(player => player.LostMatchRecords).WithOne(matchRecord => matchRecord.LoserPlayer)
            .HasForeignKey(matchRecord => matchRecord.LoserPlayerId).OnDelete(DeleteBehavior.SetNull);
    }
}