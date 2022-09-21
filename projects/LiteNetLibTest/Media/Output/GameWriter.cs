using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia.Controls;
using System.Xml.Linq;
using LiteNetLibTest.Media.Input;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Diagnostics;

namespace LiteNetLibTest.Media.Output;

/// <summary>
/// Game session writer. Converts Ogg stream to the game objects state.
/// </summary>
internal class GameWriter : IOggOutput
{
    private readonly Control[] objects;

    /// <inheritdoc />
    public int StreamSerialNo { get; }

    /// <inheritdoc />
    public byte[] StreamHeader { get; } = Array.Empty<byte>();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="streamSerialNumber">Stream serial number.</param>
    /// <param name="objects">Game objects (controls).</param>
    public GameWriter(int streamSerialNumber, Control[] objects)
    {
        StreamSerialNo = streamSerialNumber;
        this.objects = objects;
    }

    /// <inheritdoc />
    public bool Enqueue(byte[] _, Memory<byte>[] packets)
    {
        foreach (var packet in packets)
        {
            using var stream = new MemoryStream(packet.ToArray());
            var formatter = new BinaryFormatter();

            var gameObjects = (GameObject[])formatter.Deserialize(stream);
            
            
            foreach (var go in gameObjects)
            {
                var obj = objects.FirstOrDefault(o => o.Name == go.Id);
                obj.SetValue(Canvas.LeftProperty, go.Left);
                obj.SetValue(Canvas.TopProperty, go.Top);
            }
        }

        return false;
    }
}
