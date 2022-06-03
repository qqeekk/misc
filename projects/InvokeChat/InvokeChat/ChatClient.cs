namespace InvokeChat;

public class ChatClient
{
    public Guid Id { get; set; } = Guid.Empty;

    public string Name { get; set; } = "<unknown>";

    public string PlayerSessionId { get; set; }

    /// <inheritdoc />
    public override string ToString()
        => "[" + Id.ToString("N").Substring(0, 7) + "] " + Name;
}
