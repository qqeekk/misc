using System;

namespace VStarCameraZone.Entities;

/// <summary>
/// Camera credentials for basic auth.
/// </summary>
public class CameraCredentials
{
    public string Login { get; set; }

    public string Password { get; set; }

    public static CameraCredentials Empty { get; }= new(string.Empty, String.Empty);

    public CameraCredentials()
    {
    }

    public CameraCredentials(string login, string password)
    {
        Login = login;
        Password = password;
    }
}
