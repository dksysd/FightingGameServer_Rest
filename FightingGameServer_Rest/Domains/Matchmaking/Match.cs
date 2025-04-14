namespace FightingGameServer_Rest.Domains.Matchmaking;

public record Match(string PlayerId1, string PlayerSteamId1, string PlayerId2, string PlayerSteamId2, int Ping)
{
}