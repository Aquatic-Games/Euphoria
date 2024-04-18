using System;
using grabs.Graphics;
using Silk.NET.SDL;
using u4.Math;
using SdlWindow = Silk.NET.SDL.Window;

namespace u4.Engine;

public unsafe class Window : IDisposable
{
    public event OnCloseRequested CloseRequested = delegate { };
    
    private Sdl _sdl;
    private SdlWindow* _window;
    private void* _glContext;

    private string _userTitle;
    private string _engineTitle;

    public Size<int> Size
    {
        get
        {
            int w, h;
            _sdl.GetWindowSize(_window, &w, &h);

            return new Size<int>(w, h);
        }
        set => _sdl.SetWindowSize(_window, value.Width, value.Height);
    }

    public Size<int> SizeInPixels
    {
        get
        {
            int w, h;
            _sdl.GetWindowSizeInPixels(_window, &w, &h);

            return new Size<int>(w, h);
        }
    }

    public string Title
    {
        get => _userTitle;
        set
        {
            _userTitle = value;
            _sdl.SetWindowTitle(_window, _userTitle + _engineTitle);
        }
    }

    public Window(in LaunchOptions options)
    {
        _userTitle = options.WindowTitle;
        
        _sdl = Sdl.GetApi();

        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {_sdl.GetErrorS()}");

        WindowFlags flags = WindowFlags.Shown;
        
        switch (options.Api)
        {
            case GraphicsApi.D3D11:
                break;
            case GraphicsApi.OpenGL:
                _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) GLprofile.Core);
                _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 4);
                _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);
                _sdl.GLSetAttribute(GLattr.AlphaSize, 0);

                flags |= WindowFlags.Opengl;
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        _window = _sdl.CreateWindow(options.WindowTitle, Sdl.WindowposCentered, Sdl.WindowposCentered,
            options.WindowSize.Width, options.WindowSize.Height, (uint) flags);

        if (_window == null)
            throw new Exception("Failed to create SDL window.");

        if (options.Api is GraphicsApi.OpenGL or GraphicsApi.OpenGLES)
        {
            _glContext = _sdl.GLCreateContext(_window);
            _sdl.GLMakeCurrent(_window, _glContext);
        }
    }

    internal nint Hwnd
    {
        get
        {
            SysWMInfo info = new SysWMInfo();
            _sdl.GetWindowWMInfo(_window, &info);

            return info.Info.Win.Hwnd;
        }
    }

    internal string EngineTitle
    {
        get => _engineTitle;
        set
        {
            _engineTitle = value;
            _sdl.SetWindowTitle(_window, _userTitle + _engineTitle);
        }
    }

    internal void ProcessEvents()
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
                            CloseRequested();
                            break;
                    }

                    break;
                }
            }
        }
    }

    internal void CreateGLContext(out Action<int> presentFunc, out Func<string, nint> getProcAddressFunc)
    {
        presentFunc = i =>
        {
            _sdl.GLSetSwapInterval(i);
            _sdl.GLSwapWindow(_window);
        };

        getProcAddressFunc = s => (nint) _sdl.GLGetProcAddress(s);
    }

    public void Dispose()
    {
        if (_glContext != null)
            _sdl.GLDeleteContext(_glContext);
        
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
    
    public delegate void OnCloseRequested();
}