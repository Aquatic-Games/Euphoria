using System;
using System.IO;
using Euphoria.ContentBuilder.Items;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using StbImageSharp;
using PixelFormat = Silk.NET.OpenGL.PixelFormat;
using PixelType = Silk.NET.OpenGL.PixelType;

namespace Euphoria.ContentBuilder.Processors;

public unsafe class DDSProcessor : ContentProcessor<ImageContent>
{
    private readonly Sdl _sdl;
    private readonly Window* _window;
    private readonly void* _glContext;

    private readonly GL _gl;
    
    public DDSProcessor()
    {
        _sdl = Sdl.GetApi();
        _gl = GL.GetApi(s => (nint) _sdl.GLGetProcAddress(s));

        if (_sdl.Init(Sdl.InitVideo) < 0)
            throw new Exception("Failed to initialize SDL.");

        _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) ContextProfileMask.CoreProfileBit);
        _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 3);
        _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);
        
        _window = _sdl.CreateWindow("DDSProcessor", 0, 0, 0, 0, (uint) (WindowFlags.Hidden | WindowFlags.Opengl));

        if (_window == null)
            throw new Exception("Failed to create window.");

        _glContext = _sdl.GLCreateContext(_window);
        _sdl.GLMakeCurrent(_window, _glContext);
    }
    
    public override void Process(ImageContent item, string name, string outDir)
    {
        using FileStream stream = File.OpenRead(item.Path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        uint texture = _gl.CreateTexture(TextureTarget.Texture2D);
        _gl.BindTexture(TextureTarget.Texture2D, texture);

        fixed (byte* pData = result.Data)
        {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.CompressedSrgbAlphaBptcUnorm, (uint) result.Width,
                (uint) result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pData);
        }

        byte[] newData = new byte[result.Width * result.Height * 4];
        fixed (byte* pData = newData)
        {
            _gl.GetTexImage(TextureTarget.Texture2D, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pData);
        }
        
        _gl.DeleteTexture(texture);
        
        File.WriteAllBytes(Path.Combine(outDir, $"{name}.bin"), newData);
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _gl.Dispose();
        _sdl.GLDeleteContext(_glContext);
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
}