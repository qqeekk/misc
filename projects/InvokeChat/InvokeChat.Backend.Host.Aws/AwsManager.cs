using System.Text;
using Aws.GameLift;
using Aws.GameLift.Server;
using Aws.GameLift.Server.Model;

namespace InvokeChat.Backend.Host.Aws;

public sealed class AwsManager
{
    private InvokeChatServer? server;
    private readonly int port = Random.Shared.Next(12_000, 40_000);

    public void Start(string logFile)
    {
        var response = GameLiftServerAPI.InitSDK();
        ProcessResponse(response);

        response = GameLiftServerAPI.ProcessReady(new ProcessParameters
        {
            Port = port,
            OnHealthCheck = OnHealthCheck,
            OnStartGameSession = OnStartGameSession,
            OnProcessTerminate = OnProcessTerminate,
            OnUpdateGameSession = OnUpdateGameSession,
            LogParameters = new LogParameters(new List<string>
            {
                logFile,
                "log4net.log"
            })
        });
        ProcessResponse(response);

        Console.WriteLine($"Started on port {port}.");
    }

    private void OnUpdateGameSession(UpdateGameSession updategamesession)
    {
        Console.WriteLine($"OnUpdateGameSession call: reason={updategamesession.UpdateReason}.");
    }

    private void OnProcessTerminate()
    {
        Console.WriteLine("OnProcessTerminate call.");
        var response = GameLiftServerAPI.ProcessEnding();
        ProcessResponse(response);
    }

    private void OnStartGameSession(GameSession gamesession)
    {
        var sb = new StringBuilder()
            .AppendLine("OnStartGameSession call.")
            .AppendLine($"sessionId={gamesession.GameSessionId}.")
            .AppendLine($"data={gamesession.GameSessionData}");
        Console.Write(sb);

        server = new InvokeChatServer();
        server.StartLoopInThread(port);

        var response = GameLiftServerAPI.ActivateGameSession();
        ProcessResponse(response);
        Console.WriteLine($"Session {gamesession.GameSessionId} activated.");
    }

    private bool OnHealthCheck()
    {
        return true;
    }

    private void ProcessResponse(GenericOutcome outcome)
    {
        if (!outcome.Success)
        {
            throw new InvalidOperationException(outcome.Error.ToString());
        }
    }
}
