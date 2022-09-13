using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace LiteNetLibTest.Ogg;

internal class ReadWriteBuffer<TPacket>
{
    private readonly ConcurrentQueue<TPacket> packets = new();
    
    public void Enqueue(TPacket packet) => packets.Enqueue(packet);

    public async Task PollAsync(Func<TPacket, Task> callback)
    {
        while (packets.TryDequeue(out var packet))
        {
            await callback(packet);
            (packet as IDisposable)?.Dispose();
        }
    }

    public void Poll(Action<TPacket> callback)
    {
        PollAsync(p => { callback(p); return Task.CompletedTask; }).Wait();
    }

}
