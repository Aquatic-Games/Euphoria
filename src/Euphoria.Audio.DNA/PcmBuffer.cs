using System;

namespace Euphoria.Audio.DNA;

public ref struct PcmBuffer
{
    public ReadOnlySpan<byte> Data;

    public AudioFormat Format;

    public PcmBuffer(in ReadOnlySpan<byte> data, AudioFormat format)
    {
        Data = data;
        Format = format;
    }
}