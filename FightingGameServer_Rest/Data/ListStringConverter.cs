using System.Text.Json;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace FightingGameServer_Rest.Data;

public class ListStringConverter() : ValueConverter<List<string>, string>(
    v => JsonSerializer.Serialize(v, JsonSerializerOptions.Default),
    v => JsonSerializer.Deserialize<List<string>>(v, JsonSerializerOptions.Default) ?? new List<string>());