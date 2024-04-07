using Silk.NET.SDL;
using SdlWindow = Silk.NET.SDL.Window;

namespace u4.Engine;

public unsafe class Window : IDisposable
{
    private Sdl _sdl;
    private SdlWindow* _window;
    private void* _glContext;

    public Window(WindowOptions options)
    {
        _sdl = Sdl.GetApi();

        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {_sdl.GetErrorS()}");
        
        
    }

    public void Dispose()
    {
        if (_glContext != null)
            _sdl.GLDeleteContext(_glContext);
        
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
}