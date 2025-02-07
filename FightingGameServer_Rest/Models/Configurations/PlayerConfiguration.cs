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
        builder.HasKey(x => x.Id);

        builder.Property(player => player.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(player => player.Name).HasColumnName("name").HasMaxLength(20).IsRequired();
        builder.HasIndex(player => player.Name).IsUnique();

        builder.Property(player => player.ExperiencePoint).HasColumnName("experience_point").HasDefaultValue(0)
            .IsRequired();

        builder.Property(player => player.MatchCount).HasColumnName("match_count").HasDefaultValue(0).IsRequired();

        builder.Property(player => player.UserId).HasColumnName("user_id").HasDefaultValue(-1).IsRequired();
        builder.HasOne(player => player.User).WithOne(user => user.Player);
    }
}