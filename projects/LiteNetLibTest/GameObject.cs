using System;
using LiteNetLib.Utils;

namespace LiteNetLibTest;

/// <summary>
/// Main game object.
/// </summary>
[Serializable]
public class GameObject
{
    /// <summary>
    /// Control identifier (name).
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// Left X coordinate.
    /// </summary>
    public double Left { get; set; }

    /// <summary>
    /// Top Y coordinate.
    /// </summary>
    public double Top { get; set; }

    /// <inheritdoc />
    public override string ToString() => $"{Id}: {Left} {Top}";
}
