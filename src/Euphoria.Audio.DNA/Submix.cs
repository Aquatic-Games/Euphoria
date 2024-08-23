using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Euphoria.Audio.DNA.Sources;

namespace Euphoria.Audio.DNA;

public class Submix
{
    private float[] _buffer;
    
    public List<Source> Sources;

    public Submix(uint bufferSize, uint channels)
    {
        _buffer = new float[bufferSize * channels];
        Sources = new List<Source>();
    }

    internal unsafe void GetBuffer(Span<float> outBuffer)
    {
        foreach (Source source in Sources)
            source.GetBuffer(_buffer);
        
        fixed (void* pSrc = _buffer)
        fixed (void* pDst = outBuffer)
            Unsafe.CopyBlock(pDst, pSrc, (uint) outBuffer.Length * sizeof(float));
    }
}