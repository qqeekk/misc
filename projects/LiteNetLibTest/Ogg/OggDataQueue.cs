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
        foreach (var source in sources)
        {
            source.Recorded += (e, stream) => packets.Enqueue(stream);
        }
    }

    public byte[] Pop()
    {
        using var stream = new MemoryStream();

        while (packets.TryDequeue(out var packet))
        {
            FlushPages(stream, packet, force: true);
        }

        return stream.ToArray();
    }

    public static void FlushPages(Stream stream, OggStream oggStream, bool force)
    {
        while (oggStream.PageOut(out OggPage page, force))
        {
            stream.Write(page.Header, 0, page.Header.Length);
            stream.Write(page.Body, 0, page.Body.Length);
        }
    }
}
