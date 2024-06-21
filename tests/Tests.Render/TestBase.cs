using System;
using Euphoria.Render;
using grabs.Graphics;
using grabs.Graphics.D3D11;
using Silk.NET.SDL;
using u4.Math;
using Surface = grabs.Graphics.Surface;

namespace Tests.Render;

public abstract unsafe class TestBase : IDisposable
{
    private Sdl _sdl;
    private string _title;

    private Window* _window;

    public Graphics Graphics;

    protected TestBase(string title)
    {
        _title = title;
    }

    public void Run(Size<int> size, GraphicsOptions options)
    {
        _sdl = Sdl.GetApi();

        if (_sdl.Init(Sdl.InitVideo | Sdl.InitEvents) < 0)
            throw new Exception($"Failed to initialize SDL: {_sdl.GetErrorS()}");

        WindowFlags flags = WindowFlags.Shown;

        _window = _sdl.CreateWindow(_title, Sdl.WindowposCentered, Sdl.WindowposCentered, size.Width, size.Height,
            (uint) flags);

        if (_window == null)
            throw new Exception($"Failed to create SDL window: {_sdl.GetErrorS()}");

        Instance instance = new D3D11Instance();

        SysWMInfo sysWmInfo = new SysWMInfo();
        _sdl.GetWindowWMInfo(_window, &sysWmInfo);

        Surface surface = new D3D11Surface(sysWmInfo.Info.Win.Hwnd);

        Graphics = new Graphics(instance, surface, size, options);
    }

    public void Dispose()
    {
        Graphics.Dispose();
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
}