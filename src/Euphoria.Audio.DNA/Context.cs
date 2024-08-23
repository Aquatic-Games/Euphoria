using System;
using System.Diagnostics;

namespace Euphoria.Audio.DNA;

public class Context
{
    public readonly uint SampleRate;

    public readonly uint Channels;

    public readonly uint BufferSize;

    public readonly Submix Master;

    public Context(uint sampleRate, uint bufferSize = 512)
    {
        SampleRate = sampleRate;
        Channels = 2;
        BufferSize = bufferSize;

        Master = new Submix(BufferSize, Channels);
    }

    public void GetBuffer(Span<float> outBuffer)
    {
        Debug.Assert(outBuffer.Length == BufferSize * Channels, "outBuffer.Length == BufferSize * Channels");
        Master.GetBuffer(outBuffer);
    }
}