using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media.Input;

/// <summary>
/// Microphone simulator.
/// Reads PCM (.raw) file, encodes data to Ogg and sends it partially in equal intervals.
/// </summary>
public class MicrophoneSimulator : IOggInput, IAsyncDisposable
{
    /// <summary>
    /// Ogg Stream serial number.
    /// </summary>
    public const int VorbisStreamSerialNo = 999;

    private const int FrequencyMilliseconds = 200;
    private readonly int sampleRate;
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
                audioStreamReader = new PcmAudioStreamReader(pcmBytes, VorbisStreamSerialNo, sampleRate, sample: PcmSample.SixteenBit);
            }
            return audioStreamReader;
        }
    }

    /// <inheritdoc />
    public event EventHandler<OggStream> Recorded = delegate { };

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="audioPath">Physical path to a PCM (.raw) file.</param>
    /// <param name="sampleRate">Audio sample rate (hertz).</param>
    public MicrophoneSimulator(string audioPath, int sampleRate)
    {
        this.sampleRate = sampleRate;
        this.audioPath = audioPath;
    }

    /// <summary>
    /// Start emitting <see cref="Recorded"/> events in a background thread after equal time intervals.
    /// </summary>
    public void Start()
    {
        timer = new Timer(callback: Routine, state: null, dueTime: FrequencyMilliseconds, period: FrequencyMilliseconds);

        void Routine(object? state)
        {
            Recorded.Invoke(this, AudioStreamReader.NextInterval(batches: 1));
        }
    }

    /// <inheritdoc />
    public async ValueTask DisposeAsync()
    {
        if (timer != null)
        {
            await timer.DisposeAsync();
        }
    }
}
