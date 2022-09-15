using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteNetLibTest.Media.Input;
using NVorbis;
using NVorbis.Ogg;

namespace LiteNetLibTest.Media.Output;

internal class OggDataOutput
{
    private readonly byte[] headerBytes;
    private readonly float[] buffer = new float[1 << 15];
    private readonly Dictionary<int, IOggOutput> outputs = new();

    public OggDataOutput(params IOggOutput[] outputs)
    {
        this.outputs = outputs.ToDictionary(d => d.StreamSerialNo);

        var stream = new MemoryStream();
        foreach (var output in outputs)
        {
            stream.Write(output.StreamHeader);
        }
        headerBytes = stream.ToArray();
    }

    public void ReceiveBytes(byte[] bytes)
    {
        var memoryStream = new MemoryStream(headerBytes.Concat(bytes).ToArray());

        // TODO: We need PageReader class here.
        // Fork NVorbis, add as friend assembly, introduce assembly alias.
        using var stream = new ContainerReader(memoryStream, closeOnDispose: true)
        {
            NewStreamCallback = (packetProvider) =>
            {
                var decoder = new StreamDecoder(packetProvider);
                var length = decoder.Read(buffer, 0, buffer.Length);

                if (outputs.TryGetValue(packetProvider.StreamSerial, out var device))
                {
                    var bytes = new byte[length * 4];
                    Buffer.BlockCopy(buffer, 0, bytes, 0, length * 4);

                    device.Enqueue(bytes);
                    
                }
                return true;
            }
        };

        while (stream.FindNextStream());
    }
}
