using System.Diagnostics.CodeAnalysis;
using FightingGameServer_Rest.Domains.Character.Dtos;
using FightingGameServer_Rest.Domains.Player.Dtos;

namespace FightingGameServer_Rest.Domains.MatchRecord.Dtos;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class MatchRecordDto
{
    public required DateTime StartedAt { get; set; }
    public required DateTime EndedAt { get; set; }

    public required PlayerDto WinnerPlayer { get; set; }
    public required CharacterDto WinnerCharacter { get; set; }
    public required PlayerDto LoserPlayer { get; set; }
    public required CharacterDto LoserCharacter { get; set; }
}