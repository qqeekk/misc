using System;

namespace VStarCameraZone.Entities;

/// <summary>
/// Camera exception.
/// </summary>
public class CameraException : Exception
{
    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="message">Exception message.</param>
    public CameraException(string message) : base(message)
    {
    }
}
