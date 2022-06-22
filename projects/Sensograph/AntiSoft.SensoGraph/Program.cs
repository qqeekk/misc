using System.Threading.Tasks;
using McMaster.Extensions.CommandLineUtils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AntiSoft.SensoGraph.Infrastructure;

namespace AntiSoft.SensoGraph
{
    /// <summary>
    /// Entry point class.
    /// </summary>
    [Command(Name = "sensograph", UnrecognizedArgumentHandling = UnrecognizedArgumentHandling.CollectAndContinue)]
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
                .ConfigureServices(services =>
                {
                    services.AddHostedService<MetricsExecutionHostedService>();
                })
                .Build();

            // Command line processing.
            var commandLineApplication = new CommandLineApplication<Program>();
            using var scope = host.Services.CreateScope();
            commandLineApplication
                .Conventions
                .UseConstructorInjection(scope.ServiceProvider)
                .UseDefaultConventions();
            return await commandLineApplication.ExecuteAsync(args);
        }

        /// <summary>
        /// Command line application execution callback.
        /// </summary>
        /// <returns>Exit code.</returns>
        public async Task<int> OnExecuteAsync()
        {
            await host.InitAsync();
            await host.RunAsync();
            return 0;
        }
    }
}
