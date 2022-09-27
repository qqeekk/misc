using System;

namespace LiteNetLibTest.Media;

/// <summary>
/// Ogg output device (player).
/// </summary>
public interface IOggOutput
{
    /// <summary>
    /// Ogg stream serial number.
    /// </summary>
    int StreamSerialNo { get; }

    /// <summary>
    /// Enqueue byte stream to a buffer.
    /// </summary>
    /// <param name="stream">Full Ogg stream (multi-track).</param>
    /// <param name="packets">Ogg packets bytes (w/o headers).</param>
    /// <returns>True if all stream was proccessed. Otherwise, false.</returns>
    bool Enqueue(byte[] stream, Memory<byte>[] packets);
}