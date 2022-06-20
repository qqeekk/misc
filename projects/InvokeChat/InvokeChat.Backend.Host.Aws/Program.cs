using System.Reflection;
using McMaster.Extensions.CommandLineUtils;

namespace InvokeChat.Backend.Host.Aws;

[Command(Name = "chathostaws", Description = "Chat host AWS")]
[VersionOptionFromMember("--version", MemberName = nameof(GetVersion))]
[HelpOption("-?|-h|--help")]
public class Program
{
    public const string LogFile = "./log.log";

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
        var awsManager = new AwsManager();
        awsManager.Start(LogFile);
        Thread.Sleep(Timeout.Infinite);
        return 0;
    }

    private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        switch (e.ExceptionObject)
        {
            case Exception exception:
                Console.Error.WriteLine(exception.Message);
                File.AppendAllText(LogFile, exception.Message + "\n");
                break;
        }
        Environment.Exit(1);
    }
}
