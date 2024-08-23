using Euphoria.Audio.DNA;
using Euphoria.Audio.DNA.Sources;
using Silk.NET.SDL;
using Thread = System.Threading.Thread;

Context context = new Context(48000);
Source source = new SampleSource(new PcmBuffer(
    File.ReadAllBytes(@"C:\Users\ollie\Music\TESTFILES\Always There-32bitfloat.raw"),
    new AudioFormat(DataType.F32, 44100, Channels.Stereo)));

context.Master.Sources.Add(source);

unsafe
{
    Sdl sdl = Sdl.GetApi();

    if (sdl.Init(Sdl.InitAudio) < 0)
        throw new Exception(sdl.GetErrorS());

    AudioSpec spec = new AudioSpec()
    {
        Format = Sdl.AudioF32,
        Freq = 48000,
        Channels = 2,
        Samples = 512,
        Callback = new PfnAudioCallback((arg0, b, i) =>
        {
            context.GetBuffer(new Span<float>(b, i / sizeof(float)));
        })
    };

    uint device = sdl.OpenAudioDevice((byte*) null, 0, &spec, null, 0);

    if (device == 0)
        throw new Exception(sdl.GetErrorS());
    
    sdl.PauseAudioDevice(device, 0);

    while (true)
    {
        Thread.Sleep(1000);
    }
}