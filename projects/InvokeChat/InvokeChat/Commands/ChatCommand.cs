using System.Text.Json.Serialization;

namespace InvokeChat.Commands;

public class ChatCommand
{
    public enum CommandType
    {
        Connect,
        Disconnect,
        SendMessage
    }

    [JsonInclude]
    public CommandType Command { get; private set; }

    [JsonInclude]
    public Dictionary<string, string> Payload { get; private set; } = new();

    public ChatCommand()
    {
    }

    protected ChatCommand(CommandType command)
    {
        Command = command;
    }

    public ChatCommand(ChatCommand command)
    {
        Command = command.Command;
        Payload = command.Payload;
    }

    public ChatCommand Convert()
        => Command switch
        {
            CommandType.Connect => new ConnectChatCommand(this),
            CommandType.Disconnect => new DisconnectChatCommand(this),
            CommandType.SendMessage => new MessageChatCommand(this),
            _ => throw new ArgumentOutOfRangeException()
        };
}
