namespace Euphoria.Audio.DNA;

public class Context
{
    public readonly uint SampleRate;

    public readonly MixerChannel Master;

    public Context(uint sampleRate)
    {
        SampleRate = sampleRate;
    }
}