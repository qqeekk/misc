extern alias nvorbis;

using System;
using System.Buffers;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection.PortableExecutable;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using LiteNetLibTest.Media.Input;
using LiteNetLibTest.Ogg;
using NAudio.Vorbis;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;

namespace LiteNetLibTest.Media.Output;

/// <summary>
/// Ogg audio player.
/// </summary>
public class OggAudioPlayer : IOggOutput
{
    private static readonly FileStream file = new($"../../{DateTime.Now:HHmmss}-pcm.raw", FileMode.Append);

    private readonly int inOffset = 0;
    private readonly byte[] inBuffer = new byte[1 << 15];
    private readonly byte[] outBuffer = new byte[1 << 15];

    private readonly BufferedWaveProvider bufferedWaveProvider;
    private readonly IWavePlayer player;

    /// <inheritdoc />
    public int StreamSerialNo { get; }

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
            BufferDuration = TimeSpan.FromSeconds(400),
        };

        player = new WasapiOut();
        player.Init(bufferedWaveProvider);

        var memoryStream = new MemoryStream(inBuffer);
        var oggHeaderStream = PcmAudioStreamReader.InitializeStream(streamSerialNumber, sampleRate);
        OggDataInput.FlushPages(memoryStream, oggHeaderStream);
        inOffset = (int)memoryStream.Position;
    }

    /// <inheritdoc />
    public bool Enqueue(byte[] buffer, Memory<byte>[] _)
    {
        if (buffer.Any())
        {
            // Copy to stream.
            Array.Copy(buffer, 0, inBuffer, inOffset, buffer.Length);
            using var stream = new MemoryStream(inBuffer, 0, count: inOffset + buffer.Length);

            using var vorbisStream = new VorbisWaveReader(stream, closeOnDispose: true);

            try
            {
                var num = 0;
                do
                {
                    // TODO: not very reliable. Audio is stuttering.
                    // For some reason, output (.raw) file is 2 times larger than the source.
                    // Does it count header data?
                    num = vorbisStream.Read(this.outBuffer, 0, 4410 * 2);

                    file.Write(this.outBuffer, 0, num);
                    bufferedWaveProvider.AddSamples(this.outBuffer, 0, num);
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
        var stream = new MemoryStream(bytes);
        var vorbisStream = new VorbisWaveReader(stream, closeOnDispose: true);
        var player = new WaveOutEvent { DesiredLatency = 1000 };
        player.Init(vorbisStream);
        player.Play();
    }

    /// <summary>
    /// Play file contents in a separate thread.
    /// </summary>
    /// <param name="file">Physical path to Ogg-Vorbis (.ogg) file.</param>
    public void PlayPCMStatic(string file)
    {
        var bytes = File.ReadAllBytes(file);

        var player = new WaveOutEvent { DesiredLatency = 1000 };
        player.Init(bufferedWaveProvider);

        bufferedWaveProvider.AddSamples(bytes, 0, bytes.Length);
        player.Play();
    }
}
