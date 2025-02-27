using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FightingGameServer_Rest.Domains.CustomCommand.Dtos;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Global")]
public class UpdateCustomCommandRequestDto
{
    public enum ActionType
    {
        Create,
        Update,
        Delete
    }

    [JsonConverter(typeof(JsonStringEnumConverter<ActionType>))]
    public required ActionType Action { get; init; }

    public required CustomCommandDto CustomCommand { get; init; }
}