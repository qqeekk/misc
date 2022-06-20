using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using VStarCameraZone.Commands;

namespace VStarCameraZone;

/// <summary>
/// Entry point class.
/// </summary>
public class Program
{
    private static IHost host;

    /// <summary>
    /// Entry point method.
    /// </summary>
    /// <param name="args">Program arguments.</param>
    public static async Task<int> Main(string[] args)
    {
        // Init host.
        host = Host
            .CreateDefaultBuilder(args)
            .ConfigureWebHostDefaults(webBuilder =>
            {
                webBuilder.UseStartup<Startup>();
            })
            .Build();

        // Command line processing.
        var rootCommand = new RootCommand
        {
            host.Services.GetRequiredService<FindCamerasCommand>(),
            host.Services.GetRequiredService<SetIrCommand>(),
            host.Services.GetRequiredService<ShowCameraInfoCommand>(),
        };
        rootCommand.Handler = CommandHandler.Create(StartWebApplication);
        return await rootCommand.InvokeAsync(args);
    }

    /// <summary>
    /// Command line application execution callback.
    /// </summary>
    /// <returns>Exit code.</returns>
    private static async Task<int> StartWebApplication()
    {
        await host.RunAsync();
        return 0;
    }
}
