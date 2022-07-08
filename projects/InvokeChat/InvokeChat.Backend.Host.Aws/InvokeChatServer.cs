using System.Net;
using System.Net.Sockets;
using System.Reflection;
using System.Text;
using System.Text.Json;
using Aws.GameLift.Server;
using InvokeChat.Commands;

namespace InvokeChat.Backend.Host.Aws;

public sealed class InvokeChatServer
{
    private readonly object @lock = new();
    private TcpListener? serverSocket = null;
    private Thread? thread = null;

    private readonly Dictionary<ChatClient, TcpClient> clients = new();

    public Thread StartLoopInThread(int port)
    {
        thread = new Thread(() => StartLoop(port));
        thread.Start();
        return thread;
    }

    public void StartLoop(int port)
    {
        if (serverSocket != null)
        {
            return;
        }
        serverSocket = new TcpListener(IPAddress.Any, port);
        serverSocket.Start();
        Console.WriteLine($"Listen on port {port}.");

        while (true)
        {
            var tcpClient = serverSocket.AcceptTcpClient();
            lock (@lock)
            {
                var client = new ChatClient();
                clients.Add(client, tcpClient);
                Console.WriteLine("Someone connected.");

                var clientThread = new Thread(HandleClient);
                clientThread.Start(client);
            }
        }
    }

    private void HandleClient(object? obj)
    {
        var client = obj as ChatClient;
        if (client == null)
        {
            return;
        }
        TcpClient tcpClient;

        lock (@lock)
        {
            tcpClient = clients[client];
        }

        var buffer = new byte[1024 * 10];
        while (true)
        {
            var stream = tcpClient.GetStream();
            Array.Fill(buffer, (byte)0);
            int bytesCount = stream.Read(buffer, 0, buffer.Length);

            if (bytesCount == 0)
            {
                HandleCommand(client, tcpClient, new DisconnectChatCommand());
                return;
            }

            // There might be partial read, no workaround for now!
            var data = Encoding.Unicode.GetString(buffer, 0, bytesCount);
            ChatCommand? command;
            try
            {
                command = JsonSerializer.Deserialize<ChatCommand>(data);
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine(ex);
                continue;
            }
            if (command == null)
            {
                Console.WriteLine($"Cannot deserialize command! {data}.");
                continue;
            }
            HandleCommand(client, tcpClient, command);
        }
    }

    private void HandleCommand(ChatClient client, TcpClient tcpClient, ChatCommand command)
    {
        command = command.Convert();
        // ConnectChatCommand
        if (command.Command == ChatCommand.CommandType.Connect)
        {
            var connectCommand = (ConnectChatCommand)command;
            client.Id = connectCommand.Id;
            client.Name = connectCommand.Name;
            client.PlayerSessionId = connectCommand.PlayerSessionId;
            Console.WriteLine($"{client} connected with id {client.PlayerSessionId}.");
            var response = GameLiftServerAPI.AcceptPlayerSession(client.PlayerSessionId);
            AwsUtils.ProcessResponse(response);
            Broadcast(connectCommand);
        }
        // DisconnectChatCommand
        else if (command.Command == ChatCommand.CommandType.Disconnect)
        {
            lock (@lock)
            {
                clients.Remove(client);
            }
            tcpClient.Client.Shutdown(SocketShutdown.Both);
            tcpClient.Close();
            Console.WriteLine($"{client} disconnected.");
            var response = GameLiftServerAPI.RemovePlayerSession(client.PlayerSessionId);
            AwsUtils.ProcessResponse(response);
            Broadcast(command);
        }
        // MessageChatCommand
        else if (command.Command == ChatCommand.CommandType.SendMessage)
        {
            var chatCommand = (MessageChatCommand)command;
            Console.WriteLine($"Client '{chatCommand.ClientName}' sent message '{chatCommand.Message}'.");
            if (chatCommand.Message.StartsWith(@"\v"))
            {
                Send(client, new MessageChatCommand("<server>", GetVersion()));
            }
            else
            {
                Broadcast(chatCommand);
            }
        }
        // VersionChatCommand
        else if (command.Command == ChatCommand.CommandType.Version)
        {
            Broadcast(new VersionChatCommand(GetVersion()));
        }
        else
        {
            Console.WriteLine("Unknown command!");
        }
    }

    private static string GetVersion()
        => typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;

    private void Broadcast(ChatCommand command)
    {
        foreach (var client in clients.Keys)
        {
            Send(client, command);
        }
    }

    private void Send(ChatClient chatClient, ChatCommand command)
    {
        byte[] buffer = Encoding.Unicode.GetBytes(JsonSerializer.Serialize(command));

        lock (@lock)
        {
            var tcpClient = clients[chatClient];
            var stream = tcpClient.GetStream();
            stream.Write(buffer, 0, buffer.Length);
        }
    }
}
