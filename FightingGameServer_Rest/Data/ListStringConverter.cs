using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FightingGameServer_Rest.Data;

[SuppressMessage("ReSharper", "HeapView.ObjectAllocation")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class ListStringConverter() : ValueConverter<List<string>, string>(
    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
    v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());