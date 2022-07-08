using System.Globalization;
using System.Net;
using System.Text;
using Amazon.GameLift;
using Amazon.GameLift.Model;
using Amazon.Runtime;

namespace InvokeChat.Client.Aws;

public class AwsManager
{
    public string? AliasId { get; }

    public string? FleetId { get; }

    private readonly Dictionary<string, GameSession> gameSessions = new();
    private readonly Dictionary<string, PlayerSession> playerSessions = new();
    private readonly AmazonGameLiftClient gameLiftClient;

    public AwsManager(string accessKey, string secretKey, string aliasId)
    {
        // All available service URLS:
        // - https://docs.aws.amazon.com/general/latest/gr/rande.html
        // - https://docs.aws.amazon.com/general/latest/gr/gamelift.html
        var config = new AmazonGameLiftConfig
        {
            ServiceURL = @"https://gamelift.us-west-2.amazonaws.com"
        };
        AliasId = aliasId;
        gameLiftClient = new AmazonGameLiftClient(new BasicAWSCredentials(accessKey, secretKey), config);
    }

    public AwsManager()
    {
        var config = new AmazonGameLiftConfig
        {
            ServiceURL = "http://localhost:9080"
        };
        FleetId = "fleet-123";
        gameLiftClient = new AmazonGameLiftClient(new AnonymousAWSCredentials(), config);
    }

    public async Task CreateSessionAsync(CancellationToken cancellationToken)
    {
        var response = await gameLiftClient.CreateGameSessionAsync(new CreateGameSessionRequest
        {
            AliasId = AliasId,
            FleetId = FleetId,
            Name = "TestIt " + DateTime.Now.Second,
            GameSessionData = DateTime.Now.ToString(CultureInfo.InvariantCulture),
            MaximumPlayerSessionCount = 8
        }, cancellationToken);
        ValidateWebResponse(response);
        var sb = new StringBuilder()
            .AppendLine("Started new game session.");
        DumpGameSession(response.GameSession, sb);
        Console.WriteLine(sb);

        gameSessions[response.GameSession.GameSessionId] = response.GameSession;
    }

    public async Task DescribeSessionAsync(string gameSessionId, CancellationToken cancellationToken)
    {
        var session = GetGameSessionById(gameSessionId);
        if (session == null)
        {
            Console.WriteLine("Cannot find session.");
            return;
        }

        var response = await gameLiftClient.DescribeGameSessionsAsync(new DescribeGameSessionsRequest
        {
            GameSessionId = session.GameSessionId
        }, cancellationToken);
        ValidateWebResponse(response);
        foreach (var gameSession in response.GameSessions)
        {
            var sb = new StringBuilder();
            DumpGameSession(gameSession, sb);
            Console.WriteLine(sb);
        }
    }
    
    public async Task DescribeGameSessionsAsync(bool dump, CancellationToken cancellationToken)
    {
        var response = await gameLiftClient.DescribeGameSessionsAsync(new DescribeGameSessionsRequest
        {
            AliasId = AliasId,
            FleetId = FleetId
        }, cancellationToken);
        ValidateWebResponse(response);
        gameSessions.Clear();
        foreach (var gameSession in response.GameSessions)
        {
            var sb = new StringBuilder();
            DumpGameSession(gameSession, sb);
            gameSessions[gameSession.GameSessionId] = gameSession;
            if (dump)
            {
                Console.WriteLine(sb);
            }
        }
    }

    public async Task CreatePlayerSessionAsync(string gameSessionId, string playerId, CancellationToken cancellationToken)
    {
        var session = GetGameSessionById(gameSessionId);
        if (session == null)
        {
            Console.WriteLine("Cannot find session.");
            return;
        }

        var response = await gameLiftClient.CreatePlayerSessionAsync(new CreatePlayerSessionRequest
        {
            GameSessionId = session.GameSessionId,
            PlayerId = playerId
        }, cancellationToken);
        ValidateWebResponse(response);
        var sb = new StringBuilder();
        DumpPlayerSession(response.PlayerSession, sb);
        Console.WriteLine(sb);

        playerSessions[response.PlayerSession.PlayerSessionId] = response.PlayerSession;
    }

    public async Task DescribePlayerSessionsAsync(bool dump, string playerId, CancellationToken cancellationToken)
    {
        var response = await gameLiftClient.DescribePlayerSessionsAsync(new DescribePlayerSessionsRequest
        {
            PlayerId = playerId
        }, cancellationToken);
        ValidateWebResponse(response);
        var sb = new StringBuilder();
        foreach (var responsePlayerSession in response.PlayerSessions)
        {
            playerSessions[responsePlayerSession.PlayerSessionId] = responsePlayerSession;
            DumpPlayerSession(responsePlayerSession, sb);
            sb.AppendLine("-----");
        }
        if (dump)
        {
            Console.WriteLine(sb);
        }
    }

    public GameSession? GetGameSessionById(string session)
    {
        foreach (var gameSession in gameSessions)
        {
            if (gameSession.Key.EndsWith(session))
            {
                return gameSession.Value;
            }
        }
        return null;
    }

    public PlayerSession? GetPlayerSessionById(string session)
    {
        foreach (var playerSession in playerSessions)
        {
            if (playerSession.Key.EndsWith(session))
            {
                return playerSession.Value;
            }
        }
        return null;
    }

    private void DumpGameSession(GameSession gameSession, StringBuilder sb)
    {
        sb
            .AppendLine($"name={gameSession.Name}")
            .AppendLine($"host={gameSession.IpAddress}:{gameSession.Port}")
            .AppendLine($"sessionId={gameSession.GameSessionId}")
            .AppendLine($"status={gameSession.Status}")
            .AppendLine($"maxclients={gameSession.MaximumPlayerSessionCount}")
            .AppendLine($"clients={gameSession.CurrentPlayerSessionCount}");
    }

    private void DumpPlayerSession(PlayerSession playerSession, StringBuilder sb)
    {
        sb
            .AppendLine($"host={playerSession.IpAddress}:{playerSession.Port}")
            .AppendLine($"playerSessionId={playerSession.PlayerSessionId}")
            .AppendLine($"sessionId={playerSession.GameSessionId}")
            .AppendLine($"status={playerSession.Status}");
    }

    private void ValidateWebResponse(AmazonWebServiceResponse response)
    {
        if (response.HttpStatusCode != HttpStatusCode.OK)
        {
            throw new InvalidOperationException("Invalid response: " + response.HttpStatusCode);
        }
    }
}
