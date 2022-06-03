namespace InvokeChat.Commands;

public class DisconnectChatCommand : ChatCommand
{
    /// <inheritdoc />
    public DisconnectChatCommand() : base(CommandType.Disconnect)
    {
    }

    /// <inheritdoc />
    public DisconnectChatCommand(ChatCommand command) : base(command)
    {
    }
}
