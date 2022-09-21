extern alias nvorbis;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteNetLibTest.Media.Input;
using LiteNetLibTest.Ogg;
using NAudio.Vorbis;
using NAudio.Wave;

namespace LiteNetLibTest.Media.Output;

/// <summary>
/// Ogg audio player.
/// </summary>
public class OggAudioPlayer : IOggOutput
{
    private static readonly FileStream file = new($"D:/{DateTime.Now:HHmmss}-pcm.raw", FileMode.Append);

    private readonly byte[] buffer = new byte[1 << 15];
    private readonly BufferedWaveProvider bufferedWaveProvider;
    private readonly IWavePlayer player;

    /// <inheritdoc />
    public int StreamSerialNo { get; }

    /// <inheritdoc />
    public byte[] StreamHeader { get; }

    /// <summary>
    /// Constructor.
    /// </summary>
    /// <param name="streamSerialNumber">Stream serial number.</param>
    /// <param name="sampleRate">Sample rate (hertz).</param>
    public OggAudioPlayer(int streamSerialNumber, int sampleRate)
    {
        StreamSerialNo = streamSerialNumber;

        // Initialize player.
        bufferedWaveProvider = new BufferedWaveProvider(
            waveFormat: WaveFormat.CreateIeeeFloatWaveFormat(sampleRate, channels: 1))
        {
            BufferDuration = TimeSpan.FromSeconds(60),
        };

        player = new WasapiOut();
        player.Init(bufferedWaveProvider);

        var memoryStream = new MemoryStream();
        var oggHeaderStream = PcmAudioStreamReader.InitializeStream(streamSerialNumber, sampleRate);
        OggDataInput.FlushPages(memoryStream, oggHeaderStream);
        StreamHeader = memoryStream.ToArray();
    }

    /// <inheritdoc />
    public bool Enqueue(byte[] buffer, Memory<byte>[] _)
    {
        if (buffer.Any())
        {
            // Copy to stream.
            using var stream = new MemoryStream(StreamHeader.Concat(buffer).ToArray());
            using var vorbisStream = new VorbisWaveReader(stream, closeOnDispose: true);
            vorbisStream.NextStreamIndex = StreamSerialNo;

            try
            {
                var num = 0;
                do
                {
                    num = vorbisStream.Read(this.buffer, 0, 8820);
                    file.Write(this.buffer, 0, num);

                    bufferedWaveProvider.AddSamples(this.buffer, 0, num);
                }
                while (num > 0);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }

        return true;
    }

    /// <summary>
    /// Start worker thread to play buffered audio.
    /// </summary>
    public async void PlayAsync()
    {
        while (true)
        {
            await Task.Delay(millisecondsDelay: 2_000);

            var completion = new TaskCompletionSource();
            void OnPlaybackStopped(object? sender, StoppedEventArgs e)
            {
                if (e.Exception != null)
                {
                    Debug.WriteLine(e.Exception.Message);
                }

                player.PlaybackStopped -= OnPlaybackStopped;
                completion.TrySetResult();
            }

            // TODO: audio is stuttering.
            player.PlaybackStopped += OnPlaybackStopped;
            player.Play();
            await completion.Task;
        }
    }

    /// <summary>
    /// Play file contents in a separate thread.
    /// </summary>
    /// <param name="file">Physical path to Ogg-Vorbis (.ogg) file.</param>
    public void PlayStatic(string file)
    {
        var bytes = File.ReadAllBytes(file);
        var streram = new MemoryStream(bytes);
        var vorbisStream = new VorbisWaveReader(streram, closeOnDispose: true);
        var player = new WaveOutEvent { DesiredLatency = 1000 };
        player.Init(vorbisStream);
        player.Play();
    }
}
