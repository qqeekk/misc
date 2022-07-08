using System.Text.Json.Serialization;

namespace InvokeChat.Commands;

public class VersionChatCommand : ChatCommand
{
    [JsonIgnore]
    public string Version => Payload["version"];

    /// <inheritdoc />
    public VersionChatCommand(string version) : base(CommandType.Version)
    {
        Payload["version"] = version;
    }
}
