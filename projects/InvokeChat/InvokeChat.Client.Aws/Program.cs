using System.ComponentModel.DataAnnotations;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace InvokeChat.Client.Aws;

[Command(Name = "chatclientaws", Description = "Chat client AWS")]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[HelpOption("-?|-h|--help")]
public class Program
{
    [Option(Description = "Client name")]
    [Required]
    public string Name { get; } = string.Empty;

    [Option(Description = "Player GUID")]
    public string PlayerId { get; } = Guid.NewGuid().ToString();

    public static Task<int> Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
        return CommandLineApplication.ExecuteAsync<Program>(args);
    }

    private static string GetVersion()
        => typeof(Program).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? string.Empty;

    /// <summary>
    /// Command line application execution callback.
    /// </summary>
    /// <param name="app">Command line application.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Exit code.</returns>
    public async Task<int> OnExecute(CommandLineApplication app, CancellationToken cancellationToken)
    {
        var awsManager = new AwsManager();
        await awsManager.DescribeGameSessionsAsync(dump: false, cancellationToken);
        await awsManager.DescribePlayerSessionsAsync(dump: false, PlayerId, cancellationToken);
        while (true)
        {
            Console.WriteLine("Type command (h=help):");
            var line = Console.ReadLine() ?? string.Empty;
            var args = line.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (args.Length < 1)
            {
                continue;
            }
            var cmd = args[0];
            try
            {
                if (cmd == "ns")
                {
                    await awsManager.CreateSessionAsync(cancellationToken);
                }
                else if (cmd == "ds" && args.Length == 2)
                {
                    await awsManager.DescribeSessionAsync(args[1], cancellationToken);
                }
                else if (cmd == "cps" && args.Length == 2)
                {
                    await awsManager.CreatePlayerSessionAsync(args[1], PlayerId, cancellationToken);
                }
                else if (cmd == "c" && args.Length == 2)
                {
                    var playerSession = awsManager.GetPlayerSessionById(args[1]);
                    if (playerSession == null)
                    {
                        Console.WriteLine("Cannot find player session.");
                        continue;
                    }
                    var invokeChatClient = new InvokeChatClient(new ChatClient
                    {
                        Id = Guid.Parse(playerSession.PlayerId),
                        Name = Name,
                        PlayerSessionId = playerSession.PlayerSessionId
                    });
                    invokeChatClient.StartLoop(playerSession.IpAddress, playerSession.Port);
                }
                else if (cmd == "st")
                {
                    await awsManager.DescribeGameSessionsAsync(dump: true, cancellationToken);
                    await awsManager.DescribePlayerSessionsAsync(dump: true, PlayerId, cancellationToken);
                }
                else if (cmd == "h")
                {
                    Console.WriteLine("ns=new session, ds=describe session");
                    Console.WriteLine("cps=create player session");
                    Console.WriteLine("st=servers state");
                    Console.WriteLine("c=connect");
                    Console.WriteLine("h=help, q=quit");
                }
                else if (cmd ==  "q")
                {
                    return 0;
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        switch (e.ExceptionObject)
        {
            case Exception exception:
                Console.Error.WriteLine(exception.Message);
                break;
        }
        Environment.Exit(1);
    }
}