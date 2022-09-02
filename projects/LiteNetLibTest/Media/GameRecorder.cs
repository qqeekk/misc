using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using Avalonia.Controls;
using LiteNetLib.Utils;
using LiteNetLibTest.Ogg;
using OggVorbisEncoder;

namespace LiteNetLibTest.Media
{
    internal class GameRecorder : IMediaSource
    {
        public const int MetadataStreamSerialNo = 1;
        private const int SampleRate = 44100;
        
        private readonly Control[] objects;

        public event EventHandler<OggStream> Recorded = delegate { };

        public GameRecorder(Control[] objects)
        {
            this.objects = objects;
        }

        public void Start()
        {
            var data = VorbisInfo.InitVariableBitRate(MetadataStreamSerialNo, SampleRate, baseQuality: 1f);
            var dataStream = new OggStream(MetadataStreamSerialNo);
            var dataHeader = HeaderPacketBuilder.BuildInfoPacket(data);
            dataStream.PacketIn(dataHeader);

            Recorded.Invoke(this, dataStream);
        }

        public void PollState()
        {
            var data = objects
                .Select(obj => new GameObject
                {
                    Id = obj.Name,
                    Left = obj.GetValue(Canvas.LeftProperty),
                    Top = obj.GetValue(Canvas.TopProperty),
                })
                .ToArray();

            var formatter = new BinaryFormatter();
            using var stream = new MemoryStream();
            try
            {
                formatter.Serialize(stream, data);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                throw;
            }

            var dataStream = new OggStream(MetadataStreamSerialNo);
            var packet = new OggPacket(stream.ToArray(), true, 0, 0);
            dataStream.PacketIn(packet);

            Recorded.Invoke(this, dataStream);
        }

        public void AddToSkeleton(OggStream skeletonStream)
        {
            // Skeleton Fisbone(metadata)
            var metadataSkeletonFisbone = OggSkeletonBuilder.BuildFisbone(
                serialNumber: MetadataStreamSerialNo,
                headerPackets: 1,
                granuleNumerator: (ulong)SampleRate,
                granuleDenumerator: 1,
                baseGranule: 0,
                preroll: 3,
                granShift: 0,
                content: "Content-Type: text/vorbis\r\nRole: text/main\r\n");
            var metadataFisbonePacket = new OggPacket(metadataSkeletonFisbone, false, 0, 2);
            skeletonStream.PacketIn(metadataFisbonePacket);
        }
    }
}
