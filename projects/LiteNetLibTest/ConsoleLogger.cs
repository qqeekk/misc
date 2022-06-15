using System;
using LiteNetLib;

namespace LiteNetLibTest;

/// <summary>
/// Logger for LiteNetLib.
/// </summary>
public class ConsoleLogger : INetLogger
{
    /// <inheritdoc />
    public void WriteNet(NetLogLevel level, string str, params object[] args)
    {
        Console.WriteLine($"[{level}] {string.Format(str, args)}");
    }
}
