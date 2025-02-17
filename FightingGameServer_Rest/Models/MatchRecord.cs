namespace FightingGameServer_Rest.Models;

public sealed class MatchRecord
{
    public int Id { get; set; }
    public required DateTime StartedAt { get; set; }
    public required DateTime EndedAt { get; set; }
    public required int? WinnerPlayerId { get; set; }
    public required int? WinnerPlayerCharacterId { get; set; }
    public required int? LoserPlayerId { get; set; }
    public required int? LoserPlayerCharacterId { get; set; }

    public Player? WinnerPlayer { get; set; }
    public Character? WinnerPlayerCharacter { get; set; }
    public Player? LoserPlayer { get; set; }
    public Character? LoserPlayerCharacter { get; set; }
}