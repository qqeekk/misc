using LiteNetLib.Utils;

namespace LiteNetLibTest;

/// <summary>
/// Main game object.
/// </summary>
public class GameObject : INetSerializable
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
    public void Serialize(NetDataWriter writer)
    {
        writer.Put(Id);
        writer.Put(Left);
        writer.Put(Top);
    }

    /// <inheritdoc />
    public void Deserialize(NetDataReader reader)
    {
        Id = reader.GetString();
        Left = reader.GetInt();
        Top = reader.GetInt();
    }

    /// <inheritdoc />
    public override string ToString() => $"{Id}: {Left} {Top}";
}
