namespace FightingGameServer_Rest.Models;

public sealed class MatchRecord
{
    public int Id { get; init; }
    public required DateTime StartedAt { get; init; }
    public required DateTime EndedAt { get; init; }
    public required int? WinnerPlayerId { get; init; }
    public required int? WinnerPlayerCharacterId { get; init; }
    public required int? LoserPlayerId { get; init; }
    public required int? LoserPlayerCharacterId { get; init; }

    public Player? WinnerPlayer { get; init; }
    public Character? WinnerPlayerCharacter { get; init; }
    public Player? LoserPlayer { get; init; }
    public Character? LoserPlayerCharacter { get; init; }
}