extern alias nvorbis;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using nvorbis::NVorbis;
using nvorbis::NVorbis.Ogg;

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
        var handledStreams = new List<int>();

        var memoryStream = new MemoryStream(bytes);
        using var pageReader = new PageReader(memoryStream, closeOnDispose: true, _ => true);

        pageReader.Lock();
        while (pageReader.ReadNextPage())
        {
            if (handledStreams.Contains(pageReader.StreamSerial))
            {
                continue;
            }

            if (outputs.TryGetValue(pageReader.StreamSerial, out var output))
            {
                var isHandled = output.Enqueue(bytes, pageReader.GetPackets());
                if (isHandled) handledStreams.Add(pageReader.StreamSerial);
            }
        }
        pageReader.Release();
    }
}
