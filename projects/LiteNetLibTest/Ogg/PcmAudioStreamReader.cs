﻿using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Remote.Protocol.Viewport;
using OggVorbisEncoder;

namespace LiteNetLibTest.Ogg;

internal class PcmAudioStreamReader
{
    private const int WriteBufferSize = 512;
    private readonly static object _obj = new();
    
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

    public OggStream NextInterval(int bufferSize)
    {
        // Convert to ogg packets.

        lock (_obj)
        {
            var packetNo = ++this.packetNo;

            using var outputStream = new MemoryStream();
            var audioStream = new OggStream(serialNo);
            
            var from = packetNo * WriteBufferSize;
            var to = from + bufferSize;
            
            for (; from < to; from += WriteBufferSize)
            {
                if (from == samples.Length)
                {
                    processingState.WriteEndOfStream();
                }
                else
                {
                    processingState.WriteData(
                        data: new[] { samples },
                        length: WriteBufferSize,
                        read_offset: from);
                }

                while (!audioStream.Finished && processingState.PacketOut(out OggPacket packet))
                {
                    audioStream.PacketIn(packet);
                }
            }
            return audioStream;
        }
    }

    public OggStream InitializeStream()
    {
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