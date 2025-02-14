namespace FightingGameServer_Rest.Models;

public class MatchRecord
{
    public int Id { get; set; }
    public required DateTime StartedAt { get; set; }
    public required DateTime EndedAt { get; set; }
    public required int? WinnerPlayerId { get; set; }
    public required int? WinnerPlayerCharacterId { get; set; }
    public required int? LoserPlayerId { get; set; }
    public required int? LoserPlayerCharacterId { get; set; }

    public virtual Player? WinnerPlayer { get; set; }
    public virtual Character? WinnerPlayerCharacter { get; set; }
    public virtual Player? LoserPlayer { get; set; }
    public virtual Character? LoserPlayerCharacter { get; set; }
}