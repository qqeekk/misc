using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using VStarCameraZone.Entities;
using VStarCameraZone.Services;

namespace VStarCameraZone.Commands;

/// <summary>
/// Set camera IR on/off.
/// </summary>
public class SetIrCommand : CameraCommand
{
    private readonly CamerasFinder camerasFinder;
    private readonly IOptions<CredentialsDictionary> credentialsOptions;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="camerasFinder">Cameras finder.</param>
    /// <param name="credentialsOptions">Credentials map options.</param>
    public SetIrCommand(
        CamerasFinder camerasFinder,
        IOptions<CredentialsDictionary> credentialsOptions) :
        base(
            name: "set-ir",
            description: "Set IR state")
    {
        this.camerasFinder = camerasFinder;
        this.credentialsOptions = credentialsOptions;
        AddArgument(new Argument("value")
        {
            ArgumentType = typeof(bool),
            Description = "\"true\" set switch IR on, \"false\" for off"
        });
        Handler = CommandHandler
            .Create<string, bool, IConsole>(Handle);
    }

    private async Task<int> Handle(
        string ip,
        bool value,
        IConsole console)
    {
        var cameras = await camerasFinder.Find(CancellationToken.None);
        var camera = cameras.FirstOrDefault(c => c.Ip == ip);
        if (camera == null)
        {
            console.Error.WriteLine("Cannot find camera");
            return 1;
        }
        new CamerasCredentialsUpdater(silent: true).SetCredentialsByIp(camera, credentialsOptions.Value);
        await camera.SetIr(value, CancellationToken.None);
        console.Out.WriteLine($"Set IR to {value}");
        return 0;
    }
}
