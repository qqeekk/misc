using System.CommandLine;
using System.CommandLine.Invocation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VStarCameraZone.Entities;
using VStarCameraZone.Services;

namespace VStarCameraZone.Commands;

/// <summary>
/// Show more information about camera command.
/// </summary>
public class ShowCameraInfoCommand : CameraCommand
{
    private readonly CamerasFinder camerasFinder;
    private readonly IOptions<CredentialsDictionary> credentialsOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="camerasFinder">Cameras finder.</param>
    /// <param name="credentialsOptions">Credentials map options.</param>
    public ShowCameraInfoCommand(
        CamerasFinder camerasFinder,
        IOptions<CredentialsDictionary> credentialsOptions) :
        base(
            name: "show",
            description: "Show the info about camera")
    {
        this.camerasFinder = camerasFinder;
        this.credentialsOptions = credentialsOptions;
        Handler = CommandHandler.Create<string, IConsole>(Handle);
    }

    private async Task<int> Handle(string ip, IConsole console)
    {
        var cameras = await camerasFinder.Find(CancellationToken.None);
        var camera = cameras.FirstOrDefault(c => c.Ip == ip);
        if (camera != null)
        {
            console.Out.Write(camera.DumpInfo());
            new CamerasCredentialsUpdater(silent: true).SetCredentialsByIp(camera, credentialsOptions.Value);
            var parameters = await camera.GetParameters(CancellationToken.None);
            console.Out.Write(parameters.DumpInfo());
        }
        return 0;
    }
}
