using System;
using System.IO;
using Euphoria.ContentBuilder.Items;
using grabs.Graphics;
using Silk.NET.OpenGL;
using Silk.NET.SDL;
using StbImageSharp;
using PixelFormat = Silk.NET.OpenGL.PixelFormat;
using PixelType = Silk.NET.OpenGL.PixelType;

namespace Euphoria.ContentBuilder.Processors;

public unsafe class DDSProcessor : ContentProcessor<DDSContent>
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
    
    public override void Process(DDSContent item, string name, string outDir)
    {
        using FileStream stream = File.OpenRead(item.Path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        Format format = item.Format;
        bool isCompressed = format is >= Format.BC1_UNorm and <= Format.BC7_UNorm_SRGB;
        
        uint bpp = format.BitsPerPixel();
        if (isCompressed)
            bpp /= 2;
        else
            bpp /= 8;

        uint texture = _gl.CreateTexture(TextureTarget.Texture2D);
        _gl.BindTexture(TextureTarget.Texture2D, texture);

        InternalFormat iFmt = format switch
        {
            Format.R8G8B8A8_UNorm => InternalFormat.Rgba8,
            Format.BC1_UNorm => InternalFormat.CompressedRgbaS3TCDxt1Ext,
            Format.BC1_UNorm_SRGB => InternalFormat.CompressedSrgbAlphaS3TCDxt1Ext,
            Format.BC2_UNorm => InternalFormat.CompressedRgbaS3TCDxt3Ext,
            Format.BC2_UNorm_SRGB => InternalFormat.CompressedSrgbAlphaS3TCDxt3Ext,
            Format.BC3_UNorm => InternalFormat.CompressedRgbaS3TCDxt5Ext,
            Format.BC3_UNorm_SRGB => InternalFormat.CompressedSrgbAlphaS3TCDxt5Ext,
            Format.BC4_UNorm => InternalFormat.CompressedRedRgtc1,
            Format.BC4_SNorm => InternalFormat.CompressedSignedRedRgtc1,
            Format.BC5_UNorm => InternalFormat.CompressedRGRgtc2,
            Format.BC5_SNorm => InternalFormat.CompressedSignedRGRgtc2,
            Format.BC6H_UF16 => InternalFormat.CompressedRgbBptcUnsignedFloat,
            Format.BC6H_SF16 => InternalFormat.CompressedRgbBptcSignedFloat,
            Format.BC7_UNorm => InternalFormat.CompressedRgbaBptcUnorm,
            Format.BC7_UNorm_SRGB => InternalFormat.CompressedSrgbAlphaBptcUnorm,
            _ => throw new NotSupportedException($"Format {format} is not supported.")
        };

        fixed (byte* pData = result.Data)
        {
            _gl.TexImage2D(TextureTarget.Texture2D, 0, iFmt, (uint) result.Width, (uint) result.Height, 0,
                PixelFormat.Rgba, PixelType.UnsignedByte, pData);
        }
        
        byte[] newData = new byte[result.Width * result.Height * bpp];
        fixed (byte* pData = newData)
        {
            _gl.GetCompressedTexImage(TextureTarget.Texture2D, 0, pData);
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