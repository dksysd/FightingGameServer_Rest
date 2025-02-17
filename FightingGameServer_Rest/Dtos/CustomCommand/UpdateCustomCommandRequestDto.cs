using System.Diagnostics.CodeAnalysis;
using System.Text.Json.Serialization;

namespace FightingGameServer_Rest.Dtos.CustomCommand;

[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
public class UpdateCustomCommandRequestDto
{
    public enum ActionType
    {
        Create,
        Update,
        Delete
    }

    [JsonConverter(typeof(JsonStringEnumConverter<ActionType>))]
    public required ActionType Action { get; set; }

    public required CustomCommandDto CustomCommand { get; set; }
}