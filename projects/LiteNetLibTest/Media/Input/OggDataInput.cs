using System.IO;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media.Input;

/// <summary>
/// Ogg data input. Aggregates all connected devices (microphones, game recorders, etc - see <seealso cref="IOggInput"/>).
/// </summary>
internal class OggDataInput
{
    private readonly ReadWriteBuffer<OggStream> buffer = new();

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="sources">Ogg data sources.</param>
    public OggDataInput(params IOggInput[] sources)
    {
        foreach (var source in sources)
        {
            source.Recorded += (e, stream) => buffer.Enqueue(stream);
        }
    }

    /// <summary>
    /// Poll for data collected in buffer.
    /// </summary>
    /// <returns>Ogg streams bytes.</returns>
    public byte[] Flush()
    {
        using var stream = new MemoryStream();
        buffer.Poll(packet => FlushPages(stream, packet));
        return stream.ToArray();
    }

    /// <summary>
    /// Flush <see cref="OggStream"/ stream to the <see cref="OggStream"/>.
    /// </summary>
    /// <param name="stream">Target stream.</param>
    /// <param name="oggStream">Source stream.</param>
    public static void FlushPages(Stream stream, OggStream oggStream)
    {
        while (oggStream.PageOut(out OggPage page, force: true))
        {
            stream.Write(page.Header, 0, page.Header.Length);
            stream.Write(page.Body, 0, page.Body.Length);
        }
    }
}
