using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Avalonia.Controls;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media.Input;

/// <summary>
/// Game session recorder. Converts game state to Ogg packets.
/// </summary>
internal class GameRecorder : IOggInput
{
    public const int MetadataStreamSerialNo = 33;

    private readonly Control[] objects;
    private int packetNumber = 0;

    /// <inheritdoc />
    public event EventHandler<OggStream> Recorded = delegate { };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="objects">Tracked objects.</param>
    public GameRecorder(Control[] objects)
    {
        this.objects = objects;
    }

    /// <summary>
    /// Poll current game state and send it converted to Ogg format.
    /// This method triggers <see cref="Recorded"/> event.
    /// </summary>
    public void PollState()
    {
        var data = objects
            .Select(obj => new GameObject
            {
                Id = obj.Name,
                Left = obj.GetValue(Canvas.LeftProperty),
                Top = obj.GetValue(Canvas.TopProperty),
            })
            .ToArray();

        var formatter = new BinaryFormatter();
        using var stream = new MemoryStream();
        try
        {
            formatter.Serialize(stream, data);
        }
        catch (Exception ex)
        {
            Debug.WriteLine(ex.Message);
            throw;
        }

        var dataStream = new OggStream(MetadataStreamSerialNo);
        var packet = new OggPacket(stream.ToArray(), false, 0, Interlocked.Increment(ref packetNumber));
        dataStream.PacketIn(packet);

        Recorded.Invoke(this, dataStream);
    }
}
