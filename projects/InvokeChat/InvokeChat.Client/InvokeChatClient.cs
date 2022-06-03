using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using InvokeChat.Commands;

namespace InvokeChat.Client;

public class InvokeChatClient
{
    private readonly ChatClient client;

    public InvokeChatClient(ChatClient client)
    {
        this.client = client;
    }

    public void StartLoop(string host, int port)
    {
        var ip = IPAddress.Parse(host);
        var tcpClient = new TcpClient();
        tcpClient.Connect(ip, port);
        Console.WriteLine("Client connected.");
        var networkStream = tcpClient.GetStream();

        // Connect.
        var connectCommand = new ConnectChatCommand(client.Id, client.Name, client.PlayerSessionId);
        var connectBuffer = Encoding.Unicode.GetBytes(JsonSerializer.Serialize(connectCommand));
        networkStream.Write(connectBuffer, 0, connectBuffer.Length);

        var thread = new Thread(ReceiveData);
        thread.Start(tcpClient);

        while (true)
        {
            var str = Console.ReadLine();
            if (string.IsNullOrEmpty(str))
            {
                continue;
            }
            if (str == "exit")
            {
                break;
            }

            // Send chat message.
            var command = new MessageChatCommand(client.Name, str);
            var buffer = Encoding.Unicode.GetBytes(JsonSerializer.Serialize(command));
            networkStream.Write(buffer, 0, buffer.Length);
        }

        tcpClient.Client.Shutdown(SocketShutdown.Send);
        networkStream.Close();
        tcpClient.Close();

        Console.WriteLine("Disconnect from server.");
    }

    private void ReceiveData(object? obj)
    {
        var tcpClient = obj as TcpClient;
        if (tcpClient == null)
        {
            return;
        }

        var networkStream = tcpClient.GetStream();
        byte[] receivedBytes = new byte[1024 * 8];
        int byteCount;

        while ((byteCount = networkStream.Read(receivedBytes, 0, receivedBytes.Length)) > 0)
        {
            var data = Encoding.Unicode.GetString(receivedBytes, 0, byteCount);
            var command = JsonSerializer.Deserialize<ChatCommand>(data);
            if (command == null)
            {
                Console.WriteLine("Cannot deserialize command!");
                continue;
            }

            HandleCommand(command);
        }
    }

    private void HandleCommand(ChatCommand command)
    {
        command = command.Convert();
        if (command.Command == ChatCommand.CommandType.SendMessage)
        {
            var chatCommand = (MessageChatCommand)command;
            Console.WriteLine($"{chatCommand.ClientName}: {chatCommand.Message}");
        }
        else if (command.Command == ChatCommand.CommandType.Connect)
        {
            var connectCommand = (ConnectChatCommand)command;
            Console.WriteLine($"> '{connectCommand.Name}' joined!");
        }
    }
}
