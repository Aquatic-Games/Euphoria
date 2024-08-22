namespace Euphoria.Audio.DNA.Sources;

public abstract class Source
{
    internal abstract void GetBuffer(float[] outBuffer, uint sampleRate);
}