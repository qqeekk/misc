using System.IO;
using System.Linq;
using System.Threading.Tasks;
using LiteNetLibTest.Ogg;
using NAudio.Wave;

namespace LiteNetLibTest.Media;

public class OggAudioPlayer
{
    private byte[] headerBytes;
    private ReadWriteBuffer<IWaveProvider> buffer = new();
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
        var vorbisStream = new NAudio.Vorbis.VorbisWaveReader(stream, closeOnDispose: true);
        vorbisStream.NextStreamIndex = MicrophoneSimulator.VorbisStreamSerialNo;

        this.buffer.Enqueue(vorbisStream);
    }

    public async void PlayAsync()
    {
        while (!stopped)
        {
            await this.buffer.PollAsync(stream =>
            {
                var completion = new TaskCompletionSource();
                var waveOut = new NAudio.Wave.WasapiOut();
                waveOut.Init(stream);
                waveOut.PlaybackStopped += (s, e) => completion.TrySetResult();

                waveOut.Play();
                return completion.Task;
            });
        }
    }

    public void Stop()
    {
        this.stopped = true;
    }
}
