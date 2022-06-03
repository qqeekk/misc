using System.Reflection;
using McMaster.Extensions.CommandLineUtils;
using InvokeChat.Backend.Host;

[Command(Name = "chathost", Description = "Chat host")]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[HelpOption("-?|-h|--help")]
public class Program
{
    [Option(Description = "Chat host port")]
    public int Port { get; } = Random.Shared.Next(12_000, 40_000);

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
        var server = new InvokeChatServer();
        server.StartLoop(port: Port);
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
