using OggVorbisEncoder;

namespace LiteNetLibTest.Media;

public interface IOggOutput
{
    public int StreamSerialNo { get; }

    public byte[] StreamHeader { get; }

    public void Enqueue(byte[] stream);
}