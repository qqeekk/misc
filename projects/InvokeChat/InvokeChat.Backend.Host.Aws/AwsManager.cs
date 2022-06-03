using System.Text;
using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;

namespace InvokeChat.Backend.Host.Aws;

public sealed class AwsManager
{
    private InvokeChatServer? server;
    private readonly int port = Random.Shared.Next(12_000, 40_000);

    public void Start()
    {
        var response = GameLiftServerAPI.InitSDK();
        if (!response.Success)
        {
            throw new InvalidOperationException(response.Error.ToString());
        }

        response = GameLiftServerAPI.ProcessReady(new ProcessParameters
        {
            Port = port,
            OnHealthCheck = OnHealthCheck,
            OnStartGameSession = OnStartGameSession,
            OnProcessTerminate = OnProcessTerminate,
            OnUpdateGameSession = OnUpdateGameSession
        });
        if (!response.Success)
        {
            throw new InvalidOperationException(response.Error.ToString());
        }

        Console.WriteLine($"Started on port {port}.");
    }

    private void OnUpdateGameSession(UpdateGameSession updategamesession)
    {
        Console.WriteLine($"OnUpdateGameSession call: reason={updategamesession.UpdateReason}.");
    }

    private void OnProcessTerminate()
    {
        Console.WriteLine("OnProcessTerminate call.");
    }

    private void OnStartGameSession(GameSession gamesession)
    {
        gamesession.MaximumPlayerSessionCount = 20;
        var sb = new StringBuilder()
            .AppendLine("OnStartGameSession call.")
            .AppendLine($"sessionId={gamesession.GameSessionId}.")
            .AppendLine($"data={gamesession.GameSessionData}");
        Console.Write(sb);

        server = new InvokeChatServer();
        server.StartLoopInThread(port);

        var response = GameLiftServerAPI.ActivateGameSession();
        if (!response.Success)
        {
            throw new InvalidOperationException(response.Error.ToString());
        }
        Console.WriteLine($"Session {gamesession.GameSessionId} activated.");
    }

    private bool OnHealthCheck()
    {
        return true;
    }
}
