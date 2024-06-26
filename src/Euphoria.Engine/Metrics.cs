using System.Diagnostics;

namespace Euphoria.Engine;

public static class Metrics
{
    private static Stopwatch _deltaWatch;
    //private static Stopwatch _totalWatch;

    private static double _timeSinceLastFrame;

    public static double TimeSinceLastFrame => _timeSinceLastFrame;

    internal static void Initialize()
    {
        _deltaWatch = Stopwatch.StartNew();
        //_totalWatch = Stopwatch.StartNew();
    }

    internal static void Update()
    {
        _timeSinceLastFrame = _deltaWatch.Elapsed.TotalSeconds;
        _deltaWatch.Restart();
    }
}