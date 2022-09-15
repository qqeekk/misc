using System;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media;

public interface IOggInput
{
    public event EventHandler<OggStream> Recorded;
}
