using System.Collections.Generic;
using VStarCameraZone.Entities;

namespace VStarCameraZone.Services;

/// <summary>
/// The class works with camera credentials.
/// </summary>
public class CamerasCredentialsUpdater
{
    private readonly bool silent;

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="silent">Do not throw exception if credentials were not found.</param>
    public CamerasCredentialsUpdater(bool silent)
    {
        this.silent = silent;
    }

    /// <summary>
    /// Set credentials from credentials map.
    /// </summary>
    /// <param name="cameras">Cameras set.</param>
    /// <param name="credentialsDictionary">Credentials map.</param>
    public void SetCredentialsByIp(
        IEnumerable<Camera> cameras,
        CredentialsDictionary credentialsDictionary)
    {
        foreach (var camera in cameras)
        {
            SetCredentialsByIp(camera, credentialsDictionary);
        }
    }

    /// <summary>
    /// Set credentials from credentials map.
    /// </summary>
    /// <param name="camera">Camera instance.</param>
    /// <param name="credentialsDictionary">Credentials map.</param>
    public void SetCredentialsByIp(Camera camera, CredentialsDictionary credentialsDictionary)
    {
        if (credentialsDictionary.TryGetValue(camera.Ip, out CameraCredentials credentials))
        {
            camera.Credentials = credentials;
        }
        else if (!silent)
        {
            throw new CameraException($"Cannot find credentials for camera {camera.Ip}.");
        }
    }
}
