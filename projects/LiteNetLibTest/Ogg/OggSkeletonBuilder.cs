using System;
using System.Diagnostics;
using OggVorbisEncoder;

namespace LiteNetLibTest.Ogg;

internal class OggSkeletonBuilder
{
    public static byte[] BuildFishead(ulong segmentLength, ulong contentOffset)
    {
        const ushort SkeletonVersionMajor = 4;
        const ushort SkeletonVersionMinor = 0;
        const string DateTimeFormat = "yyyyMMddTHHmmss.FFFZ";

        var buffer = new EncodeBuffer();
        var presentationTimeNumerator = 0ul;
        var presentationTimeDenominator = 1000ul;
        var baseTimeNumerator = 0ul;
        var baseTimeDenominator = 1000ul;
        DateTime? utc = null;

        buffer.WriteString("fishead\0");
        buffer.Write(SkeletonVersionMajor, 8 * sizeof(ushort));
        buffer.Write(SkeletonVersionMinor, 8 * sizeof(ushort));
        buffer.Write(presentationTimeNumerator, 8 * sizeof(ulong));
        buffer.Write(presentationTimeDenominator, 8 * sizeof(ulong));
        buffer.Write(baseTimeNumerator, 8 * sizeof(ulong));
        buffer.Write(baseTimeDenominator, 8 * sizeof(ulong));

        if (utc != null)
        {
            buffer.WriteString(utc?.ToString(DateTimeFormat));
        }
        else
        {
            for (var i = 0; i < DateTimeFormat.Length; i++)
            {
                buffer.Write(0, 8);
            }
        }

        buffer.Write(segmentLength);
        buffer.Write(contentOffset);
        return buffer.GetBytes();
    }

    public static byte[] BuildFisbone(
        uint serialNumber,
        uint headerPackets,
        ulong granuleNumerator,
        ulong granuleDenumerator,
        ulong baseGranule,
        uint preroll,
        byte granShift,
        string content)
    {
        const uint MessageHeaderOffset = 44;

        var buffer = new EncodeBuffer();
        buffer.WriteString("fisbone\0");
        buffer.Write(MessageHeaderOffset, 8 * sizeof(uint));
        buffer.Write(serialNumber, 8 * sizeof(uint));
        buffer.Write(headerPackets, 8 * sizeof(uint));
        buffer.Write(granuleNumerator, 8 * sizeof(ulong));
        buffer.Write(granuleDenumerator, 8 * sizeof(ulong));
        buffer.Write(baseGranule, 8 * sizeof(ulong));
        buffer.Write(preroll, 8 * sizeof(uint));
        buffer.Write(granShift, 8 * sizeof(byte));
        buffer.Write(0, 3 * 8 * sizeof(byte)); // reserved.
        buffer.WriteString(content);

        return buffer.GetBytes();
    }
}

internal static class EncodeBufferExtensions
{
    public static void Write(this EncodeBuffer buffer, ulong value, int bits = 64)
    {
        Debug.Assert(bits >= 64);

        // Split into 32-bit integers.
        var l = (uint)(value >> 32);
        var r = (uint)(value & uint.MaxValue);

        // Print in reverse.
        buffer.Write(r, bits - 32);
        buffer.Write(l, 32);
    }
}
