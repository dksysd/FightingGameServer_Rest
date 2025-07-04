﻿using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace FightingGameServer_Rest.Domains.MatchRecord.Dtos;

[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class GetMatchRecordInfoRequestDto
{
    [Required]
    public required string PlayerName { get; init; }
}