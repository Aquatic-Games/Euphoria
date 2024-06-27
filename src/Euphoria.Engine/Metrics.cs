using System;
using System.Diagnostics;

namespace Euphoria.Engine;

public static class Metrics
{
    private static Action<int> _onFpsUpdate;
    
    private static Stopwatch _deltaWatch;
    //private static Stopwatch _totalWatch;

    private static double _deltaTime;

    private static double _accumulator;
    private static int _framesAccumulator;
    private static int _fps;
    private static ulong _totalFrames;

    public static double TimeSinceLastFrame => _deltaTime;

    public static int FramesPerSecond => _fps;
    
    public static ulong TotalFrames => _totalFrames;

    internal static void Initialize(Action<int> onFpsUpdate)
    {
        _onFpsUpdate = onFpsUpdate;
        
        _deltaWatch = Stopwatch.StartNew();
        //_totalWatch = Stopwatch.StartNew();
    }

    internal static bool Update(double targetDelta)
    {
        _deltaTime = _deltaWatch.Elapsed.TotalSeconds;

        if (_deltaTime < targetDelta)
            return true;
        
        _deltaWatch.Restart();

        _totalFrames++;
        _framesAccumulator++;

        _accumulator += _deltaTime;
        if (_accumulator >= 1.0)
        {
            _accumulator -= 1.0;
            _fps = _framesAccumulator;
            _framesAccumulator = 0;
            _onFpsUpdate(_fps);
        }

        return false;
    }
}