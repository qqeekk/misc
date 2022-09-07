using System;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media;

public interface IMediaSource
{
    public event EventHandler<OggStream> Recorded;
    public OggStream GetLogicalStreamHeader();
    public OggPacket GetSkeletonFisbone();
}