using System;
using System.Collections.Concurrent;

namespace LiteNetLibTest.Ogg;

/// <summary>
/// FIFO data buffer.
/// </summary>
/// <typeparam name="TPacket"></typeparam>
internal class ReadWriteBuffer<TPacket>
{
    private readonly ConcurrentQueue<TPacket> packets = new();

    /// <summary>
    /// Enqueue one packet.
    /// </summary>
    /// <param name="packet"></param>
    public void Enqueue(TPacket packet) => packets.Enqueue(packet);

    /// <summary>
    /// Poll for data.
    /// </summary>
    /// <param name="callback">Packet handler.</param>
    public void Poll(Action<TPacket> callback)
    {
        while (packets.TryDequeue(out var packet))
        {
            callback(packet);
            (packet as IDisposable)?.Dispose();
        }
    }
}
