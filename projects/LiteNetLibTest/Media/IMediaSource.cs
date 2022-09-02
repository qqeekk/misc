using System;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media;

public interface IMediaSource
{
    public event EventHandler<OggStream> Recorded;

    public void AddToSkeleton(OggStream skeletonStream);
}