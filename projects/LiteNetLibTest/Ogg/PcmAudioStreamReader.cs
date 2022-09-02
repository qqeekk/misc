using System;
using System.IO;
using System.Threading.Tasks;
using OggVorbisEncoder;

namespace LiteNetLibTest.Ogg;

internal class PcmAudioStreamReader : IAsyncDisposable
{
    private const int WriteBufferSize = 512;

    private readonly int serialNo;
    private readonly BinaryReader streamReader;
    private readonly PcmSample sample;
    private readonly VorbisInfo header;

    public PcmAudioStreamReader(BinaryReader streamReader, int serialNo, int rate, PcmSample sample)
    {
        this.streamReader = streamReader;
        this.serialNo = serialNo;
        this.sample = sample;
        header = VorbisInfo.InitVariableBitRate(channels: 1, sampleRate: rate, baseQuality: 0.5f);
    }

    public OggStream NextInterval(int bufferSize)
    {
        // Convert to floats.
        var samples = GetFloats(streamReader.ReadBytes(bufferSize));

        // Convert to ogg packets.
        var audioStream = new OggStream(serialNo);
        var audioProcessingState = ProcessingState.Create(header);
        for (int readIndex = 0; readIndex <= samples.Length; readIndex += WriteBufferSize)
        {
            if (readIndex == samples.Length)
            {
                audioProcessingState.WriteEndOfStream();
            }
            else
            {
                audioProcessingState.WriteData(new[] { samples }, WriteBufferSize, readIndex);
            }

            while (audioProcessingState.PacketOut(out OggPacket packet))
            {
                audioStream.PacketIn(packet);
            }
        }
        return audioStream;
    }

    public OggStream InitializeStream()
    {
        // Voibis.
        var audioStream = new OggStream(serialNo);
        var audioHeader = HeaderPacketBuilder.BuildInfoPacket(header);
        audioStream.PacketIn(audioHeader);

        // Comments
        var commentsPacket = HeaderPacketBuilder.BuildCommentsPacket(new Comments());
        audioStream.PacketIn(commentsPacket);

        // Books
        var booksPacket = HeaderPacketBuilder.BuildBooksPacket(header);
        audioStream.PacketIn(booksPacket);

        return audioStream;
    }

    private float[] GetFloats(byte[] bytes)
    {
        var sampleNumbers = bytes.Length / (int)sample;

        // Ensure that sample buffer is aligned to write chunk size.
        var allignedSampleNumbers = (sampleNumbers / WriteBufferSize) * WriteBufferSize;
        var samples = new float[allignedSampleNumbers];

        for (int sampleNumber = 0; sampleNumber < allignedSampleNumbers; sampleNumber++)
        {
            var sampleIndex = sampleNumber * (int)sample;
            samples[sampleNumber] = sample switch
            {
                PcmSample.EightBit => ByteToSample(bytes[sampleIndex]),
                PcmSample.SixteenBit => ShortToSample((short)(bytes[sampleIndex + 1] << 8 | bytes[sampleIndex])),
                _ => throw new NotImplementedException(),
            };
        }

        return samples;

        static float ByteToSample(short pcmValue) => pcmValue / 128f;
        static float ShortToSample(short pcmValue) => pcmValue / 32768f;
    }

    public ValueTask DisposeAsync()
    {
        streamReader.Dispose();
        return ValueTask.CompletedTask;
    }
}

public enum PcmSample : int
{
    EightBit = 1,
    SixteenBit = 2
}