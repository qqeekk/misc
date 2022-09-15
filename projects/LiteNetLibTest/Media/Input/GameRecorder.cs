using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading;
using Avalonia.Controls;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media.Input;

internal class GameRecorder : IOggInput
{
    public const int MetadataStreamSerialNo = 33;
    private const int SampleRate = 44100;

    private readonly Control[] objects;
    private int packetNumber = 0;

    public event EventHandler<OggStream> Recorded = delegate { };

    public GameRecorder(Control[] objects)
    {
        this.objects = objects;
    }

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

    public OggStream GetLogicalStreamHeader()
    {
        var data = VorbisInfo.InitVariableBitRate(MetadataStreamSerialNo, SampleRate, baseQuality: 1f);
        var dataStream = new OggStream(MetadataStreamSerialNo);
        var dataHeader = HeaderPacketBuilder.BuildInfoPacket(data);
        dataStream.PacketIn(dataHeader);

        return dataStream;
    }

    public OggPacket GetSkeletonFisbone()
    {
        // Skeleton Fisbone(metadata)
        var metadataSkeletonFisbone = OggSkeletonBuilder.BuildFisbone(
            serialNumber: MetadataStreamSerialNo,
            headerPackets: 1,
            granuleNumerator: SampleRate,
            granuleDenumerator: 1,
            baseGranule: 0,
            preroll: 3,
            granShift: 0,
            content: "Content-Type: text/vorbis\r\nRole: text/main\r\n");
        return new OggPacket(metadataSkeletonFisbone, false, 0, 2);
    }
}
