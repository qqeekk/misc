using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media.Input;

public class MicrophoneSimulator : IOggInput, IAsyncDisposable
{
    public const int FrequencyMilliseconds = 200;
    public const int VorbisStreamSerialNo = 999;

    private readonly string audioPath;
    private Timer? timer;
    private PcmAudioStreamReader? audioStreamReader;

    private PcmAudioStreamReader AudioStreamReader
    {
        get
        {
            if (audioStreamReader == null)
            {
                var pcmBytes = File.ReadAllBytes(audioPath);
                audioStreamReader = new PcmAudioStreamReader(pcmBytes, VorbisStreamSerialNo, SampleRate, sample: PcmSample.SixteenBit);
            }
            return audioStreamReader;
        }
    }

    public int SampleRate { get; }

    public event EventHandler<OggStream> Recorded = delegate { };

    public MicrophoneSimulator(string audioPath, int sampleRate)
    {
        SampleRate = sampleRate;
        this.audioPath = audioPath;
    }

    public void Start()
    {
        timer = new Timer(callback: Routine, state: null, dueTime: FrequencyMilliseconds, period: FrequencyMilliseconds);

        void Routine(object? state)
        {
            Recorded.Invoke(this, AudioStreamReader.NextInterval(packets: 1));
        }
    }

    public async ValueTask DisposeAsync()
    {
        if (timer != null)
        {
            await timer.DisposeAsync();
        }
    }

    public OggPacket GetSkeletonFisbone()
    {
        // Skeleton Fisbone (audio)
        var vorbisSkeletonFisbone = OggSkeletonBuilder.BuildFisbone(
            serialNumber: VorbisStreamSerialNo,
            headerPackets: 1,
            granuleNumerator: (ulong)SampleRate,
            granuleDenumerator: 1,
            baseGranule: 0,
            preroll: 3,
            granShift: 0,
            content: "Content-Type: audio/vorbis\r\nRole: audio/main\r\n");

        return new OggPacket(vorbisSkeletonFisbone, false, 0, 1);
    }
}
