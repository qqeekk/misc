using System;
using System.Collections.Concurrent;
using System.IO;
using LiteNetLibTest.Media;
using OggVorbisEncoder;

namespace LiteNetLibTest.Ogg;

internal class OggDataQueue
{
    private readonly ConcurrentQueue<OggStream> packets = new();

    public OggDataQueue(params IMediaSource[] sources)
    {
        // =========================================================
        // HEADER
        // =========================================================
        // Vorbis streams begin with three headers; the initial header (with
        // most of the codec setup parameters) which is mandated by the Ogg
        // bitstream spec.  The second header holds any comment fields.  The
        // third header holds the bitstream codebook.

        // Skeleton Fishead
        var skeletonStream = new OggStream(1);
        var skeletonFishead = OggSkeletonBuilder.BuildFishead(0, 0);
        var skeletonFisheadPacket = new OggPacket(skeletonFishead, false, 0, 0);

        skeletonStream.PacketIn(skeletonFisheadPacket);
        foreach (var source in sources)
        {
            source.Recorded += (e, stream) => packets.Enqueue(stream);
            source.AddToSkeleton(skeletonStream);
        }

        packets.Enqueue(skeletonStream);
    }

    public byte[] Pop()
    {
        using var stream = new MemoryStream();

        while (packets.TryDequeue(out var packet))
        {
            var force = true;
            while (packet.PageOut(out OggPage page, force))
            {
                stream.Write(page.Header, 0, page.Header.Length);
                stream.Write(page.Body, 0, page.Body.Length);
                force = false;
            }
        }

        return stream.ToArray();
    }
}
