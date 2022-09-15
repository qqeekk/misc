using System.IO;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media.Input;

internal class OggDataInput
{
    private readonly ReadWriteBuffer<OggStream> buffer = new();

    public OggDataInput(params IOggInput[] sources)
    {
        foreach (var source in sources)
        {
            source.Recorded += (e, stream) => buffer.Enqueue(stream);
        }
    }

    public byte[] Flush()
    {
        using var stream = new MemoryStream();
        buffer.Poll(packet => FlushPages(stream, packet, force: true));
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
