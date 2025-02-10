using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FightingGameServer_Rest.Models.Configurations;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
public class SkillConfiguration : IEntityTypeConfiguration<Skill>
{
    public void Configure(EntityTypeBuilder<Skill> builder)
    {
        builder.ToTable("skill");
        builder.HasKey(skill => skill.Id);

        builder.Property(skill => skill.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(skill => skill.Name).HasColumnName("name").HasMaxLength(20).IsRequired();
        builder.HasIndex(skill => skill.Name).IsUnique();

        builder.Property(skill => skill.Description).HasColumnName("description").HasMaxLength(255).IsRequired();

        builder.Property(skill => skill.IsPassive).HasColumnName("is_passive").HasDefaultValue(false).IsRequired();

        builder.Property(skill => skill.CoolTime).HasColumnName("cool_time").IsRequired();

        builder.Property(skill => skill.Range).HasColumnName("range").HasDefaultValue(0).IsRequired();

        builder.Property(skill => skill.HealthCoefficient).HasColumnName("health_coefficient").HasDefaultValue(0)
            .IsRequired();

        builder.Property(skill => skill.StrengthCoefficient).HasColumnName("strength_coefficient").HasDefaultValue(0)
            .IsRequired();

        builder.Property(skill => skill.DexterityCoefficient).HasColumnName("dexterity_coefficient").HasDefaultValue(0)
            .IsRequired();

        builder.Property(skill => skill.IntelligenceCoefficient).HasColumnName("intelligence_coefficient")
            .HasDefaultValue(0).IsRequired();

        builder.Property(skill => skill.MoveSpeedCoefficient).HasColumnName("move_speed_coefficient").HasDefaultValue(0)
            .IsRequired();

        builder.Property(skill => skill.AttackSpeedCoefficient).HasColumnName("attack_speed_coefficient")
            .HasDefaultValue(0).IsRequired();

        builder.Property(skill => skill.DefaultCommand).HasColumnName("default_command").HasMaxLength(20).IsRequired();

        builder.HasIndex(skill => new { skill.DefaultCommand, skill.CharacterId }).IsUnique();

        builder.Property(skill => skill.CharacterId).HasColumnName("character_id").IsRequired();
        builder.HasOne(skill => skill.Character).WithMany(character => character.Skills);

        builder.HasMany(skill => skill.CustomCommands).WithOne(command => command.Skill)
            .HasForeignKey(command => command.SkillId).OnDelete(DeleteBehavior.Cascade);
    }
}