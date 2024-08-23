using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace Euphoria.Audio.DNA.Sources;

public class SampleSource : Source
{
    private byte[] _sampleData;
    
    private AudioFormat _sampleFormat;

    private int _tempPos;
    
    public override uint ActiveVoices { get; }

    public SampleSource(PcmBuffer sample)
    {
        _sampleData = sample.Data.ToArray();
        _sampleFormat = sample.Format;
    }

    internal override void Initialize(uint sampleRate)
    {
        
    }

    internal override unsafe void GetBuffer(Span<float> outBuffer)
    {
        Debug.Assert(_sampleFormat.DataType == DataType.F32, "_sampleFormat.DataType == DataType.F32");
        Debug.Assert(_sampleFormat.Channels == Channels.Stereo, "_sampleFormat.Channels == Channels.Stereo");
        
        fixed (void* pSrc = _sampleData)
        fixed (void* pDst = outBuffer)
            Unsafe.CopyBlock(pDst, (byte*) pSrc + _tempPos, (uint) outBuffer.Length * sizeof(float));

        _tempPos += outBuffer.Length * sizeof(float);
    }
}