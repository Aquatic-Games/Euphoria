using System;

namespace Euphoria.Audio.DNA.Sources;

public abstract class Source
{
    public abstract uint ActiveVoices { get; }

    internal abstract void Initialize(uint sampleRate);
    
    internal abstract void GetBuffer(Span<float> outBuffer);
}