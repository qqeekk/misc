using System.Text.Json.Serialization;

namespace InvokeChat.Commands;

public class MessageChatCommand : ChatCommand
{
    [JsonIgnore]
    public string Message => Payload["message"];

    [JsonIgnore]
    public string ClientName => Payload["client_name"];

    /// <inheritdoc />
    public MessageChatCommand(string clientName, string message) : base(CommandType.SendMessage)
    {
        Payload["client_name"] = clientName;
        Payload["message"] = message;
    }

    /// <inheritdoc />
    public MessageChatCommand(ChatCommand command) : base(command)
    {
    }
}
