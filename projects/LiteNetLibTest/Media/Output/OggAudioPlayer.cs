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

public class OggAudioPlayer : IOggOutput
{
    private readonly byte[] buffer = new byte[1 << 16];
    private readonly BufferedWaveProvider bufferedWaveProvider;
    private readonly IWavePlayer player;

    public int StreamSerialNo { get; }
    public byte[] StreamHeader { get; }

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
        OggDataInput.FlushPages(memoryStream, oggHeaderStream, force: true);
        StreamHeader = memoryStream.ToArray();
    }

    public void Enqueue(byte[] buffer)
    {
        if (buffer.Any())
        {
            // Copy to stream.
            using var stream = new MemoryStream(StreamHeader.Concat(buffer).ToArray());
            using var vorbisStream = new VorbisWaveReader(stream, closeOnDispose: true);
            vorbisStream.NextStreamIndex = StreamSerialNo;

            try
            {
                var num = vorbisStream.Read(this.buffer, 0, this.buffer.Length);
                bufferedWaveProvider.AddSamples(this.buffer, 0, num);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
            }
        }
    }

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
