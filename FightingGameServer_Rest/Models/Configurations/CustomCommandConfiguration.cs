using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FightingGameServer_Rest.Models.Configurations;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class CustomCommandConfiguration : IEntityTypeConfiguration<CustomCommand>
{
    public void Configure(EntityTypeBuilder<CustomCommand> builder)
    {
        builder.ToTable("custom_command");
        builder.HasKey(customCommand => customCommand.Id);

        builder.Property(customCommand => customCommand.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(customCommand => customCommand.Command).HasColumnName("command").HasMaxLength(20).IsRequired();

        builder.Property(customCommand => customCommand.PlayerId).HasColumnName("player_id").IsRequired();

        builder.Property(customCommand => customCommand.CharacterId).HasColumnName("character_id").IsRequired();

        builder.Property(customCommand => customCommand.SkillId).HasColumnName("skill_id").IsRequired();

        builder.HasIndex(customCommand =>
            new { customCommand.PlayerId, customCommand.CharacterId, customCommand.SkillId });

        builder.HasOne(customCommand => customCommand.Player).WithMany(player => player.CustomCommands);
        builder.HasOne(customCommand => customCommand.Character).WithMany(character => character.CustomCommands);
        builder.HasOne(customCommand => customCommand.Skill).WithMany(skill => skill.CustomCommands);
    }
}