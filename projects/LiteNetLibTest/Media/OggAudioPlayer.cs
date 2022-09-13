using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteNetLibTest.Ogg;
using NAudio.CoreAudioApi;
using NAudio.Vorbis;
using NAudio.Wave;

namespace LiteNetLibTest.Media;

public class OggAudioPlayer
{
    private readonly byte[] headerBytes;
    private readonly ReadWriteBuffer<VorbisWaveReader> buffer = new();

    private bool stopped;

    public OggAudioPlayer(IMediaSource microphone)
    {
        var stream = new MemoryStream();
        OggDataRecorder.FlushPages(stream, microphone.GetLogicalStreamHeader(), force: true);
        headerBytes = stream.ToArray();
    }

    public void Enqueue(byte[] buffer)
    {
        // Copy to stream.
        var stream = new MemoryStream(headerBytes.Concat(buffer).ToArray());
        var vorbisStream = new VorbisWaveReader(stream, closeOnDispose: true);
        vorbisStream.NextStreamIndex = MicrophoneSimulator.VorbisStreamSerialNo;

        this.buffer.Enqueue(vorbisStream);
    }

    public async void PlayAsync()
    {
        while (!stopped)
        {
            await this.buffer.PollAsync(vorbisStream =>
            {
                var completion = new TaskCompletionSource();
                void OnPlaybackStopped(object? sender, StoppedEventArgs e)
                {
                    if (e.Exception != null)
                    {
                        Debug.WriteLine(e.Exception.Message);
                    }

                    completion.TrySetResult();
                }

                var speaker = new WasapiOut(AudioClientShareMode.Shared, latency: 0);
                speaker.Init(vorbisStream);
                speaker.PlaybackStopped += OnPlaybackStopped;
                speaker.Play();

                return completion.Task;
            });
        }
    }

    public void Stop()
    {
        this.stopped = true;
    }
}
