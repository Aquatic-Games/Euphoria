using System;
using Euphoria.Math;
using Euphoria.Render;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using grabs.Graphics.GL43;
using Silk.NET.SDL;
using Surface = grabs.Graphics.Surface;

namespace Tests.Render;

public abstract unsafe class TestBase : IDisposable
{
    private Sdl _sdl;
    private string _title;
    private bool _alive;

    private Window* _window;
    private void* _glContext;

    public Graphics Graphics;

    protected TestBase(string title)
    {
        _title = title;
    }

    protected virtual void Initialize() { }

    protected virtual void Update(float dt) { }

    protected virtual void Draw() { }

    public void Run(Size<int> size, GraphicsApi api, GraphicsOptions options)
    {
        _sdl = Sdl.GetApi();

        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {_sdl.GetErrorS()}");

        WindowFlags flags = WindowFlags.Shown;

        switch (api)
        {
            case GraphicsApi.D3D11:
                break;
            case GraphicsApi.OpenGL:
                flags |= WindowFlags.Opengl;
                
                _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 4);
                _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);
                _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) GLprofile.Core);
                break;
            case GraphicsApi.OpenGLES:
            case GraphicsApi.Vulkan:
            default:
                throw new ArgumentOutOfRangeException(nameof(api), api, null);
        }

        _window = _sdl.CreateWindow(_title + $" - {api}", Sdl.WindowposCentered, Sdl.WindowposCentered, size.Width,
            size.Height, (uint) flags);

        if (_window == null)
            throw new Exception($"Failed to create SDL window: {_sdl.GetErrorS()}");

        switch (api)
        {
            case GraphicsApi.D3D11:
            {
                Instance instance = new D3D11Instance();

                SysWMInfo sysWmInfo = new SysWMInfo();
                _sdl.GetWindowWMInfo(_window, &sysWmInfo);

                Surface surface = new D3D11Surface(sysWmInfo.Info.Win.Hwnd);

                Graphics = new Graphics(instance, surface, size, options);
                break;
            }

            case GraphicsApi.OpenGL:
            {
                _glContext = _sdl.GLCreateContext(_window);
                _sdl.GLMakeCurrent(_window, _glContext);

                Instance instance = new GL43Instance(s => (nint) _sdl.GLGetProcAddress(s));
                Surface surface = new GL43Surface(i =>
                {
                    _sdl.GLSetSwapInterval(i);
                    _sdl.GLSwapWindow(_window);
                });

                Graphics = new Graphics(instance, surface, size, options);

                break;
            }
        }
        

        Initialize();
        
        _alive = true;
        while (_alive)
        {
            Event winEvent;
            while (_sdl.PollEvent(&winEvent) != 0)
            {
                switch ((EventType) winEvent.Type)
                {
                    case EventType.Windowevent:
                    {
                        switch ((WindowEventID) winEvent.Window.Event)
                        {
                            case WindowEventID.Close:
                                _alive = false;
                                break;
                        }

                        break;
                    }
                }
            }
            
            Update(1 / 60.0f);
            Draw();
            
            Graphics.Present();
        }
    }

    public void Dispose()
    {
        Graphics.Dispose();
        
        if (_glContext != null)
            _sdl.GLDeleteContext(_glContext);
        
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
}