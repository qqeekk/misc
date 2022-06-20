using System.CommandLine;
using System.CommandLine.Invocation;
using System.Threading;
using System.Threading.Tasks;
using VStarCameraZone.Services;

namespace VStarCameraZone.Commands;

/// <summary>
/// Find cameras command.
/// </summary>
public class FindCamerasCommand : Command
{
    private readonly CamerasFinder camerasFinder;

    /// <inheritdoc />
    public FindCamerasCommand(
        CamerasFinder camerasFinder) :
        base(
            name: "find",
            description: "Show the list of cameras in the network")
    {
        this.camerasFinder = camerasFinder;
        Handler = CommandHandler.Create<IConsole>(Handle);
    }

    private async Task<int> Handle(IConsole console)
    {
        var cameras = await camerasFinder.Find(CancellationToken.None);
        foreach (var camera in cameras)
        {
            console.Out.Write("--------");
            console.Out.Write(camera.DumpInfo());
        }
        return 0;
    }
}
