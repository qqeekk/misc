using System;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media;

/// <summary>
/// Ogg input device (recorder).
/// </summary>
public interface IOggInput
{
    /// <summary>
    /// New data recorded event.
    /// </summary>
    public event EventHandler<OggStream> Recorded;
}
