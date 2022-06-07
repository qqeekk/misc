using System.ComponentModel.DataAnnotations;
using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using InvokeChat;

[Command(Name = "chatclient", Description = "Chat client")]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[HelpOption("-?|-h|--help")]
public class Program
{
    [Option(Description = "Client name")]
    [Required]
    public string Name { get; } = string.Empty;

    [Option(Description = "Host")]
    public string Host { get; } = "127.0.0.1";

    [Option(Description = "Host port")]
    public int Port { get; } = 12345;

    [Required]
    public string PlayerSessionId { get; }

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
    /// <returns>Exit code.</returns>
    public int OnExecute(CommandLineApplication app)
    {
        var server = new InvokeChatClient(new ChatClient
        {
            Id = Guid.NewGuid(),
            Name = Name,
            PlayerSessionId = PlayerSessionId
        });
        server.StartLoop(Host, Port);
        return 0;
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
