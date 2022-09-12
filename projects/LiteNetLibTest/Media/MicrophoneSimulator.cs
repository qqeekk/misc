using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media;

internal class MicrophoneSimulator : IMediaSource, IAsyncDisposable
{
	public const int FrequencyMilliseconds = 20;
    public const int VorbisStreamSerialNo = 999;

    private readonly string audioPath;
	private readonly int sampleRate;
	private Timer? timer;
	private PcmAudioStreamReader? audioStreamReader;

	private PcmAudioStreamReader AudioStreamReader
	{
		get
		{
			if (audioStreamReader == null)
			{
                var pcmBytes = File.ReadAllBytes(audioPath);
                this.audioStreamReader = new PcmAudioStreamReader(pcmBytes, VorbisStreamSerialNo, sampleRate, sample: PcmSample.SixteenBit);
			}
            return audioStreamReader;
        }
    }


    public event EventHandler<OggStream> Recorded = delegate { };

	public MicrophoneSimulator(string audioPath, int sampleRate)
	{
		this.audioPath = audioPath;
		this.sampleRate = sampleRate;
	}

	public void Start()
	{
		this.timer = new Timer(callback: Routine, state: null, dueTime: FrequencyMilliseconds, period: FrequencyMilliseconds);

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

	public OggStream GetLogicalStreamHeader()
	{
        return AudioStreamReader.InitializeStream();
    }

    public OggPacket GetSkeletonFisbone()
	{
        // Skeleton Fisbone (audio)
        var vorbisSkeletonFisbone = OggSkeletonBuilder.BuildFisbone(
            serialNumber: VorbisStreamSerialNo,
            headerPackets: 1,
            granuleNumerator: (ulong)sampleRate,
            granuleDenumerator: 1,
            baseGranule: 0,
            preroll: 3,
            granShift: 0,
            content: "Content-Type: audio/vorbis\r\nRole: audio/main\r\n");

        return new OggPacket(vorbisSkeletonFisbone, false, 0, 1);
    }
}
