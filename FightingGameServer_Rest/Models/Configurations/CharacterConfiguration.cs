using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FightingGameServer_Rest.Models.Configurations;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "HeapView.BoxingAllocation")]
public class CharacterConfiguration : IEntityTypeConfiguration<Character>
{
    public void Configure(EntityTypeBuilder<Character> builder)
    {
        builder.ToTable("character");
        builder.HasKey(character => character.Id);

        builder.Property(character => character.Id).HasColumnName("id").ValueGeneratedOnAdd();
        builder.Property(character => character.Name).HasColumnName("name").HasMaxLength(10).IsRequired();
        builder.HasIndex(character => character.Name).IsUnique();

        builder.Property(character => character.Health).HasColumnName("health").HasDefaultValue(0).IsRequired();

        builder.Property(character => character.Strength).HasColumnName("strength").HasDefaultValue(0).IsRequired();

        builder.Property(character => character.Dexterity).HasColumnName("dexterity").HasDefaultValue(0).IsRequired();

        builder.Property(character => character.Intelligence).HasColumnName("intelligence").HasDefaultValue(0)
            .IsRequired();

        builder.Property(character => character.MoveSpeed).HasColumnName("move_speed").HasDefaultValue(0).IsRequired();

        builder.Property(character => character.AttackSpeed).HasColumnName("attack_speed").HasDefaultValue(0)
            .IsRequired();

        builder.HasMany(character => character.Skills).WithOne(skill => skill.Character)
            .HasForeignKey(skill => skill.CharacterId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(character => character.CustomCommands).WithOne(command => command.Character)
            .HasForeignKey(command => command.CharacterId).OnDelete(DeleteBehavior.Cascade);

        builder.HasMany(character => character.WonMatchRecords).WithOne(matchRecord => matchRecord.WinnerPlayerCharacter)
            .HasForeignKey(matchRecord => matchRecord.WinnerPlayerCharacterId).OnDelete(DeleteBehavior.SetNull);

        builder.HasMany(character => character.LostMatchRecords).WithOne(matchRecord => matchRecord.LoserPlayerCharacter)
            .HasForeignKey(matchRecord => matchRecord.LoserPlayerCharacterId).OnDelete(DeleteBehavior.SetNull);
    }
}