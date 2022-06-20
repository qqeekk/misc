using System.CommandLine;

namespace VStarCameraZone.Commands;

/// <summary>
/// Base class for camera commands that has camera IP address.
/// </summary>
public class CameraCommand : Command
{
    /// <inheritdoc />
    protected CameraCommand(string name, string description = null) :
        base(name, description)
    {
        AddArgument(new Argument("ip")
        {
            ArgumentType = typeof(string),
            Description = "camera IP address"
        });
    }
}
