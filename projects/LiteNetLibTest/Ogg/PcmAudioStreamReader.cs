using System;
using System.IO;
using OggVorbisEncoder;

namespace LiteNetLibTest.Ogg;

internal class PcmAudioStreamReader
{
    private const int WriteBufferSize = 1 << 14;
    
    private readonly object _obj = new();
    private readonly float[] samples;
    private readonly int serialNo;
    private readonly PcmSample sample;
    private readonly VorbisInfo header;
    private readonly ProcessingState processingState;
    private volatile int packetNo = -1;

    public PcmAudioStreamReader(byte[] bytes, int serialNo, int rate, PcmSample sample)
    {
        this.serialNo = serialNo;
        this.sample = sample;
        samples = GetFloats(bytes);
        header = VorbisInfo.InitVariableBitRate(channels: 1, sampleRate: rate, baseQuality: 0.5f);
        processingState = ProcessingState.Create(header);
    }

    public OggStream NextInterval(int packets)
    {
        // TODO: order is not guaranteed. see "lock convoy".
        // This can lead to wrong granule position in the final queue.
        lock (_obj)
        {
            var packetNo = ++this.packetNo;

            using var outputStream = new MemoryStream();
            var audioStream = new OggStream(serialNo);
            
            for (var from = 0; from < packets; from++)
            {
                var offset = (packetNo + from) * WriteBufferSize;
                if (offset == samples.Length)
                {
                    processingState.WriteEndOfStream();
                }
                else
                {
                    processingState.WriteData(
                        data: new[] { samples },
                        length: WriteBufferSize,
                        read_offset: offset);
                }

                while (!audioStream.Finished && processingState.PacketOut(out OggPacket packet))
                {
                    audioStream.PacketIn(packet);
                }
            }
            return audioStream;
        }
    }

    public static OggStream InitializeStream(int serialNo, int rate)
    {
        var header = VorbisInfo.InitVariableBitRate(channels: 1, sampleRate: rate, baseQuality: 0.5f);

        // Voibis.
        var audioStream = new OggStream(serialNo);
        var audioHeader = HeaderPacketBuilder.BuildInfoPacket(header);
        audioStream.PacketIn(audioHeader);

        // Comments
        var comments = new Comments();
        comments.AddTag("ARTIST", "TEST");
        var commentsPacket = HeaderPacketBuilder.BuildCommentsPacket(comments);
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
}

public enum PcmSample : int
{
    EightBit = 1,
    SixteenBit = 2
}