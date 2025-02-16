using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;

namespace FightingGameServer_Rest.Dtos.CustomCommand;

public class UpdateCustomCommandRequestDto
{
    public enum ActionType
    {
        Create, Update, Delete
    }
    
    [JsonConverter(typeof(JsonStringEnumConverter<ActionType>))]
    public required ActionType Action { get; set; }
    public required CustomCommandDto CustomCommand { get; set; }
}