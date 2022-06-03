using System.Text.Json.Serialization;

namespace InvokeChat.Commands;

public class ConnectChatCommand : ChatCommand
{
    [JsonIgnore]
    public Guid Id => Guid.Parse(Payload["id"]);

    [JsonIgnore]
    public string Name => Payload["name"];

    [JsonIgnore]
    public string PlayerSessionId => Payload["psid"];

    /// <inheritdoc />
    public ConnectChatCommand(Guid id, string name, string playerSessionId) : base(CommandType.Connect)
    {
        Payload["id"] = id.ToString();
        Payload["name"] = name;
        Payload["psid"] = playerSessionId;
    }

    /// <inheritdoc />
    public ConnectChatCommand(ChatCommand command) : base(command)
    {
    }
}
