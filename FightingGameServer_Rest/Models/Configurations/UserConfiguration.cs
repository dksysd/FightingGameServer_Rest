using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FightingGameServer_Rest.Models.Configurations;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class UserConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder.ToTable("user");
        builder.HasKey(user => user.Id);

        builder.Property(user => user.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(user => user.LoginId).HasColumnName("login_id").HasMaxLength(20).IsRequired();
        builder.HasIndex(user => user.LoginId).IsUnique();

        builder.Property(user => user.LoginPassword).HasColumnName("login_password").HasMaxLength(255).IsRequired();

        builder.Property(user => user.Salt).HasColumnName("salt").HasMaxLength(255).IsRequired();

        builder.HasOne(user => user.Player).WithOne(player => player.User)
            .HasForeignKey<Player>(player => player.UserId).OnDelete(DeleteBehavior.Cascade);
    }
}