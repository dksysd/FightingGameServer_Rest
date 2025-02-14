using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FightingGameServer_Rest.Models.Configurations;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
public class MatchRecordConfiguration : IEntityTypeConfiguration<MatchRecord>
{
    public void Configure(EntityTypeBuilder<MatchRecord> builder)
    {
        builder.ToTable("match_record");
        builder.HasKey(matchRecord => matchRecord.Id);

        builder.Property(matchRecord => matchRecord.Id).HasColumnName("id").ValueGeneratedOnAdd();

        builder.Property(matchRecord => matchRecord.StartedAt).HasColumnName("started_at").HasColumnType("timestamp")
            .IsRequired();

        builder.Property(matchRecord => matchRecord.EndedAt).HasColumnName("ended_at").HasColumnType("timestamp")
            .IsRequired();

        builder.Property(matchRecord => matchRecord.WinnerPlayerId).HasColumnName("winner_player_id").IsRequired(false);

        builder.Property(matchRecord => matchRecord.WinnerPlayerCharacterId).HasColumnName("winner_player_character_id")
            .IsRequired(false);

        builder.Property(matchRecord => matchRecord.LoserPlayerId).HasColumnName("loser_player_id").IsRequired(false);

        builder.Property(matchRecord => matchRecord.LoserPlayerCharacterId).HasColumnName("loser_player_character_id")
            .IsRequired(false);

        builder.HasOne(matchRecord => matchRecord.WinnerPlayer).WithMany(player => player.WonMatchRecords);

        builder.HasOne(matchRecord => matchRecord.WinnerPlayerCharacter)
            .WithMany(character => character.WonMatchRecords);

        builder.HasOne(matchRecord => matchRecord.LoserPlayer).WithMany(player => player.LostMatchRecords);

        builder.HasOne(matchRecord => matchRecord.LoserPlayerCharacter)
            .WithMany(character => character.LostMatchRecords);

        builder.ToTable(table =>
            table.HasCheckConstraint("CK_MatchRecord_EndedAtGreaterThanStartedAt", "ended_at > started_at"));
    }
}