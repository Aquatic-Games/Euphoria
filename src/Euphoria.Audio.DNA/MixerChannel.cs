using Euphoria.Audio.DNA.Sources;

namespace Euphoria.Audio.DNA;

public class MixerChannel : Source
{
    private float[] _buffer;
    
    public List<Source> Sources;

    public MixerChannel()
    {
        _buffer = new float[512 * 2];
        Sources = new List<Source>();
    }

    internal override void GetBuffer(float[] outBuffer, uint sampleRate)
    {
        throw new NotImplementedException();
    }
}