using System;

namespace LiteNetLibTest.Media;

public interface IOggOutput
{
    public int StreamSerialNo { get; }

    public byte[] StreamHeader { get; }

    public bool Enqueue(byte[] stream, Memory<byte>[] packets);
}