using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media;

internal class MicrophoneSimulator : IMediaSource, IAsyncDisposable
{
	public const int FrequencyMilliseconds = 200;
    public const int VorbisStreamSerialNo = 999;

    private readonly string audioPath;
	private readonly int sampleRate;
	private Timer? timer;
	private PcmAudioStreamReader? audioStreamReader;

	public event EventHandler<OggStream> Recorded = delegate { };

	public MicrophoneSimulator(string audioPath, int sampleRate)
	{
		this.audioPath = audioPath;
		this.sampleRate = sampleRate;
	}

	public void Start()
	{
		var fileStream = new FileStream(audioPath, FileMode.Open, FileAccess.Read);
        var binaryReader = new BinaryReader(fileStream, System.Text.Encoding.UTF8, leaveOpen: false);
		this.audioStreamReader = new PcmAudioStreamReader(binaryReader, VorbisStreamSerialNo, sampleRate, sample: PcmSample.SixteenBit);

		Recorded.Invoke(this, audioStreamReader.InitializeStream());
		this.timer = new Timer(callback: Routine, state: null, dueTime: FrequencyMilliseconds, period: FrequencyMilliseconds);

		void Routine(object? state)
		{
			Recorded.Invoke(this, audioStreamReader.NextInterval(bufferSize: 512));
		}
    }

	public async ValueTask DisposeAsync()
	{
		if (timer != null)
		{
			await timer.DisposeAsync();
		}

		if (audioStreamReader != null)
		{
			await audioStreamReader.DisposeAsync();
		}
	}

	public void AddToSkeleton(OggStream skeletonStream)
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

        var vorbisFisbonePacket = new OggPacket(vorbisSkeletonFisbone, false, 0, 1);
        skeletonStream.PacketIn(vorbisFisbonePacket);
    }
}
