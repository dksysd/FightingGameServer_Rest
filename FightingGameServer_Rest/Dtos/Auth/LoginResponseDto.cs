﻿namespace FightingGameServer_Rest.Dtos.Auth;

public class LoginResponseDto
{
    public required string AccessToken { get; set; }
    public required string RefreshToken { get; set; }
}