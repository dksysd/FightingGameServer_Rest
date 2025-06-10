using System.ComponentModel.DataAnnotations;

namespace FightingGameServer_Rest.Models.Chatbot;

public class ChatbotSession
{
    public string SessionId { get; set; } = string.Empty;
    public string PlayerId { get; set; } = string.Empty;
    public string CharacterRole { get; set; } = string.Empty;
    public string OpponentRole { get; set; } = string.Empty;
    public string Language { get; set; } = "korean";
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    public DateTime LastActivity { get; set; } = DateTime.UtcNow;
}

public class ChatMessage
{
    [Required] public string Message { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
    public MessageType Type { get; set; } = MessageType.Chat;
}

public class ChatResponse
{
    public string Speech { get; set; } = string.Empty;
    public string Emotion { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
    public ResponseType Type { get; set; } = ResponseType.Chat;
}

public class GameStateAnalysisRequest
{
    [Required] public string OpponentActions { get; set; } = string.Empty;
    public string SessionId { get; set; } = string.Empty;
}

public class GameStateAnalysisResponse
{
    public string Analysis { get; set; } = string.Empty;
    public bool Success { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}

public class InitSessionRequest
{
    [Required] public string CharacterRole { get; set; } = string.Empty;
    [Required] public string OpponentRole { get; set; } = string.Empty;
    public string Language { get; set; } = "korean";
    public string? SessionId { get; set; }
}

public class InitSessionResponse
{
    public bool Success { get; set; }
    public string SessionId { get; set; } = string.Empty;
    public string ErrorMessage { get; set; } = string.Empty;
}

public enum MessageType
{
    Chat,
    Analysis,
    InitSession,
    EndSession,
    Error,
    Ping
}

public enum ResponseType
{
    Chat,
    Analysis,
    SessionCreated,
    SessionEnded,
    Error,
    Pong
}

public static class ChatbotCharacters
{
    public const string Vargon = "바르곤"; // 외계인
    public const string Naktis = "나크티스"; // 하피
    public const string Kagetsu = "카게츠"; // 인간

    public static readonly string[] ValidCharacters = { Vargon, Naktis, Kagetsu };

    public static bool IsValidCharacter(string character)
    {
        return ValidCharacters.Contains(character);
    }
}

// WebSocket 메시지 래퍼
public class WebSocketMessage
{
    public MessageType Type { get; set; }
    public object Data { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

public class WebSocketResponse
{
    public ResponseType Type { get; set; }
    public object Data { get; set; } = new();
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}