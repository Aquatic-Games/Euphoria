using System;
using System.Numerics;
using Euphoria.Math;
using grabs.Graphics;
using Silk.NET.SDL;
using SdlWindow = Silk.NET.SDL.Window;

namespace Euphoria.Engine;

public unsafe class Window : IDisposable
{
    public event OnCloseRequested CloseRequested = delegate { };

    public event OnKeyDown KeyDown = delegate { };

    public event OnKeyUp KeyUp = delegate { };

    public event OnMouseButtonDown MouseButtonDown = delegate { };

    public event OnMouseButtonUp MouseButtonUp = delegate { };

    public event OnMouseMove MouseMove = delegate { };

    public event OnMouseScroll MouseScroll = delegate { };

    public event OnTextInput TextInput = delegate { };

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

                case EventType.Keydown:
                {
                    KeyboardEvent keyboard = winEvent.Key;

                    if (keyboard.Repeat == 0)
                        KeyDown(SdlKeycodeToEuphoriaKey((uint) keyboard.Keysym.Sym));
                    
                    break;
                }

                case EventType.Keyup:
                {
                    KeyboardEvent keyboard = winEvent.Key;
                    KeyUp(SdlKeycodeToEuphoriaKey((uint) keyboard.Keysym.Sym));
                    break;
                }

                case EventType.Mousebuttondown:
                {
                    MouseButtonEvent button = winEvent.Button;
                    MouseButtonDown((MouseButton) button.Button);
                    break;
                }

                case EventType.Mousebuttonup:
                {
                    MouseButtonEvent button = winEvent.Button;
                    MouseButtonUp((MouseButton) button.Button);
                    break;
                }

                case EventType.Mousemotion:
                {
                    MouseMotionEvent motion = winEvent.Motion;
                    MouseMove(new Vector2(motion.X, motion.Y), new Vector2(motion.Xrel, motion.Yrel));
                    break;
                }

                case EventType.Mousewheel:
                {
                    MouseWheelEvent wheel = winEvent.Wheel;
                    MouseScroll(new Vector2(wheel.X, wheel.Y));
                    break;
                }

                case EventType.Textinput:
                {
                    TextInputEvent input = winEvent.Text;
                    for (int i = 0; input.Text[i] != 0; i++)
                        TextInput((char) input.Text[i]);
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

    // Again copied straight from Pie source.
    private static Key SdlKeycodeToEuphoriaKey(uint keycode)
    {
        const uint scancodeMask = 1 << 30;
        
        return keycode switch
        {
            '\r' => Key.Return,
            '\x1B' => Key.Escape,
            '\b' => Key.Backspace,
            '\t' => Key.Tab,
            ' ' => Key.Space,
            '#' => Key.Hash,
            '\'' => Key.Apostrophe,
            ',' => Key.Comma,
            '-' => Key.Minus,
            '.' => Key.Period,
            '/' => Key.ForwardSlash,
            '0' => Key.Num0,
            '1' => Key.Num1,
            '2' => Key.Num2,
            '3' => Key.Num3,
            '4' => Key.Num4,
            '5' => Key.Num5,
            '6' => Key.Num6,
            '7' => Key.Num7,
            '8' => Key.Num8,
            '9' => Key.Num9,
            ';' => Key.Semicolon,
            '=' => Key.Equals,
            
            '[' => Key.LeftBracket,
            ']' => Key.RightBracket,
            '\\' => Key.Backslash,
            
            '`' => Key.Backquote,
            
            'a' => Key.A,
            'b' => Key.B,
            'c' => Key.C,
            'd' => Key.D,
            'e' => Key.E,
            'f' => Key.F,
            'g' => Key.G,
            'h' => Key.H,
            'i' => Key.I,
            'j' => Key.J,
            'k' => Key.K,
            'l' => Key.L,
            'm' => Key.M,
            'n' => Key.N,
            'o' => Key.O,
            'p' => Key.P,
            'q' => Key.Q,
            'r' => Key.R,
            's' => Key.S,
            't' => Key.T,
            'u' => Key.U,
            'v' => Key.V,
            'w' => Key.W,
            'x' => Key.X,
            'y' => Key.Y,
            'z' => Key.Z,
            
            '\x7F' => Key.Delete,
            
            57 | scancodeMask => Key.CapsLock,
            
            58 | scancodeMask => Key.F1,
            59 | scancodeMask => Key.F2,
            60 | scancodeMask => Key.F3,
            61 | scancodeMask => Key.F4,
            62 | scancodeMask => Key.F5,
            63 | scancodeMask => Key.F6,
            64 | scancodeMask => Key.F7,
            65 | scancodeMask => Key.F8,
            66 | scancodeMask => Key.F9,
            67 | scancodeMask => Key.F10,
            68 | scancodeMask => Key.F11,
            69 | scancodeMask => Key.F12,
            104 | scancodeMask => Key.F13,
            105 | scancodeMask => Key.F14,
            106 | scancodeMask => Key.F15,
            107 | scancodeMask => Key.F16,
            108 | scancodeMask => Key.F17,
            109 | scancodeMask => Key.F18,
            110 | scancodeMask => Key.F19,
            111 | scancodeMask => Key.F20,
            112 | scancodeMask => Key.F21,
            113 | scancodeMask => Key.F22,
            114 | scancodeMask => Key.F23,
            115 | scancodeMask => Key.F24,
            
            70 | scancodeMask => Key.PrintScreen,
            71 | scancodeMask => Key.ScrollLock,
            72 | scancodeMask => Key.Pause,
            73 | scancodeMask => Key.Insert,
            
            74 | scancodeMask => Key.Home,
            75 | scancodeMask => Key.PageUp,
            77 | scancodeMask => Key.End,
            78 | scancodeMask => Key.PageDown,
            79 | scancodeMask => Key.Right,
            80 | scancodeMask => Key.Left,
            81 | scancodeMask => Key.Down,
            82 | scancodeMask => Key.Up,
            
            83 | scancodeMask => Key.NumLock,
            84 | scancodeMask => Key.KeypadDivide,
            85 | scancodeMask => Key.KeypadMultiply,
            86 | scancodeMask => Key.KeypadSubtract,
            87 | scancodeMask => Key.KeypadAdd,
            88 | scancodeMask => Key.KeypadEnter,
            
            89 | scancodeMask => Key.Keypad1,
            90 | scancodeMask => Key.Keypad2,
            91 | scancodeMask => Key.Keypad3,
            92 | scancodeMask => Key.Keypad4,
            93 | scancodeMask => Key.Keypad5,
            94 | scancodeMask => Key.Keypad6,
            95 | scancodeMask => Key.Keypad7,
            96 | scancodeMask => Key.Keypad8,
            97 | scancodeMask => Key.Keypad9,
            98 | scancodeMask => Key.Keypad0,
            99 | scancodeMask => Key.KeypadDecimal,
            
            101 | scancodeMask => Key.Menu,
            103 | scancodeMask => Key.KeypadEqual,

            224 | scancodeMask => Key.LeftControl,
            225 | scancodeMask => Key.LeftShift,
            226 | scancodeMask => Key.LeftAlt,
            227 | scancodeMask => Key.LeftSuper,
            228 | scancodeMask => Key.RightControl,
            229 | scancodeMask => Key.RightShift,
            230 | scancodeMask => Key.RightAlt,
            231 | scancodeMask => Key.RightSuper,

            _ => Key.Unknown
        };
    }
    
    public delegate void OnCloseRequested();

    public delegate void OnKeyDown(Key key);

    public delegate void OnKeyUp(Key key);

    public delegate void OnMouseButtonDown(MouseButton button);

    public delegate void OnMouseButtonUp(MouseButton button);

    public delegate void OnMouseMove(Vector2 position, Vector2 delta);

    public delegate void OnMouseScroll(Vector2 scroll);

    public delegate void OnTextInput(char c);
}