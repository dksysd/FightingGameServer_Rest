namespace FightingGameServer_Rest.Models;

public class MatchRecord
{
    public int Id { get; set; }
    public required DateTime StartedAt { get; set; }
    public required DateTime EndedAt { get; set; }
    public int? WinnerPlayerId { get; set; }
    public int? WinnerPlayerCharacterId { get; set; }
    public int? LoserPlayerId { get; set; }
    public int? LoserPlayerCharacterId { get; set; }

    public virtual Player? WinnerPlayer { get; set; }
    public virtual Character? WinnerPlayerCharacter { get; set; }
    public virtual Player? LoserPlayer { get; set; }
    public virtual Character? LoserPlayerCharacter { get; set; }
}