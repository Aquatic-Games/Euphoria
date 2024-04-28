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

        Console.WriteLine("Initializing SDL.");
        if (_sdl.Init(Sdl.InitVideo) < 0)
            throw new Exception("Failed to initialize SDL.");

        _sdl.GLSetAttribute(GLattr.ContextProfileMask, (int) ContextProfileMask.CoreProfileBit);
        _sdl.GLSetAttribute(GLattr.ContextMajorVersion, 3);
        _sdl.GLSetAttribute(GLattr.ContextMinorVersion, 3);
        
        Console.WriteLine("Creating hidden SDL window.");
        _window = _sdl.CreateWindow("DDSProcessor", 0, 0, 0, 0, (uint) (WindowFlags.Hidden | WindowFlags.Opengl));

        if (_window == null)
            throw new Exception("Failed to create window.");

        Console.WriteLine("Creating GL context.");
        _glContext = _sdl.GLCreateContext(_window);
        _sdl.GLMakeCurrent(_window, _glContext);
    }
    
    public override void Process(DDSContent item, string name, string outDir)
    {
        Console.WriteLine("Loading image.");
        using FileStream stream = File.OpenRead(item.Path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        Format format = item.Format;
        Console.WriteLine($"format: {format}");
        
        bool isCompressed = format is >= Format.BC1_UNorm and <= Format.BC7_UNorm_SRGB;
        Console.WriteLine($"isCompressed: {isCompressed}");
        
        bool genMips = item.GenerateMips;
        Console.WriteLine($"genMips: {genMips}");
        
        Console.WriteLine("Beginning DDS texture conversion.");

        Console.WriteLine("Creating texture.");
        uint texture = _gl.GenTexture();
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
        Console.WriteLine($"iFmt: {iFmt}");

        // Upload texture data and generate mipmaps.
        // Note that if the texture is compressed we upload the data as standard RGBA data. Reasons explained below.
        fixed (byte* pData = result.Data)
        {
            Console.WriteLine("Uploading texture data.");
            _gl.TexImage2D(TextureTarget.Texture2D, 0, isCompressed && genMips ? InternalFormat.Rgba8 : iFmt,
                (uint) result.Width, (uint) result.Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, pData);
        }

        if (genMips)
        {
            Console.WriteLine("Generating mipmaps.");
            _gl.GenerateMipmap(TextureTarget.Texture2D);
        }

        int numMips = genMips ? (int) float.Floor(float.Log2(int.Max(result.Width, result.Height))) + 1 : 1;

        // Here is a lovely bit of code that I hate with a passion but I can't see any other way of doing it without
        // using a CPU resizing library which I don't want to do right now.
        // You can't generate mipmaps for compressed textures. So, if a texture is compressed, we upload the data as
        // standard uncompressed RGBA data, and generate mipmaps.
        // Then, we create a new texture, then get the data for each mip level from the uncompressed texture, and upload
        // it to the new texture, making sure to upload it in our compressed format.
        // We then delete the original texture and set the original texture's handle to the new one.
        // It's horrible but it works and doesn't involve a CPU resizer (which would work fine, I just can't be bothered
        // to install another library, but I might do that at some point anyway)
        if (isCompressed && genMips)
        {
            Console.WriteLine("Beginning uncompressed->compressed texture conversion.");
            Console.WriteLine("Creating texture2");
            uint texture2 = _gl.GenTexture();
            
            uint newWidth = (uint) result.Width;
            uint newHeight = (uint) result.Height;
            byte[] newTexData = new byte[newWidth * newHeight * 4];
            
            for (int i = 0; i < numMips; i++)
            {
                fixed (byte* pData = newTexData)
                {
                    Console.WriteLine($"Getting uncompressed data at level {i}.");
                    
                    _gl.BindTexture(TextureTarget.Texture2D, texture);
                    _gl.GetTexImage(TextureTarget.Texture2D, i, PixelFormat.Rgba, PixelType.UnsignedByte, pData);
                    
                    Console.WriteLine($"Uploading uncompressed->compressed data at level {i}.");
                    _gl.BindTexture(TextureTarget.Texture2D, texture2);
                    _gl.TexImage2D(TextureTarget.Texture2D, i, iFmt, newWidth, newHeight, 0, PixelFormat.Rgba,
                        PixelType.UnsignedByte, pData);
                }

                newWidth = uint.Max(1, newWidth / 2);
                newHeight = uint.Max(1, newHeight / 2);
            }
            
            Console.WriteLine("Deleting original texture.");
            _gl.DeleteTexture(texture);
            texture = texture2;
            
            Console.WriteLine("Texture conversion completed.");
        }

        byte[][] texData = new byte[numMips][];

        uint width = (uint) result.Width;
        uint height = (uint) result.Height;
        
        Console.WriteLine("Beginning data export.");
        
        // Get each mip level for the texture and upload it to the corresponding mip level in the byte array.
        for (int i = 0; i < numMips; i++)
        {
            // TODO: I'd like to work out why I have to do *this* in particular for compressed textures.
            // Something in my math or understanding is wrong here.
            // But this works so it'll do for the moment. (From MS docs)
            uint size;
            if (isCompressed)
                size = uint.Max(1, (width + 3) >> 2) * uint.Max(1, (height + 3) >> 2) * format.BitsPerPixel() * 2;
            else
                size = GrabsUtils.CalculatePitch(format, width) * height;
            
            texData[i] = new byte[size];

            fixed (byte* pData = texData[i])
            {
                Console.WriteLine($"Getting texture data for level {i}.");
                if (isCompressed)
                    _gl.GetCompressedTexImage(TextureTarget.Texture2D, i, pData);
                else
                    _gl.GetTexImage(TextureTarget.Texture2D, i, PixelFormat.Rgba, PixelType.UnsignedByte, pData);
            }

            width = uint.Max(1, width / 2);
            height = uint.Max(1, height / 2);
        }

        Console.WriteLine("Deleting texture.");
        _gl.DeleteTexture(texture);
        
        Console.WriteLine("DDS texture conversion complete.");
        
        Console.WriteLine("Begin DDS file.");
        using MemoryStream memStream = new MemoryStream();
        using BinaryWriter writer = new BinaryWriter(memStream);
        
        // 'DDS '
        writer.Write(0x20534444); // dwMagic
        
        // DDS_HEADER
        Console.WriteLine("Writing DDS header.");
        
        writer.Write(124); // dwSize

        uint flags = 0x1 | 0x2 | 0x4 | 0x8 | 0x1000;
        
        if (genMips)
            flags |= 0x20000;
        if (isCompressed)
            flags |= 0x80000;
        
        writer.Write(flags); // dwFlags
        
        writer.Write(result.Height); // dwHeight
        writer.Write(result.Width); // dwWidth
        
        // dwPitchOrLinearSize
        if (isCompressed)
            writer.Write(texData[0].Length);
        else
            writer.Write(GrabsUtils.CalculatePitch(format, (uint) result.Width));

        writer.Write(0); // dwDepth
        writer.Write(numMips); // dwMipMapCount;

        // TODO: Could put message in here?
        ReadOnlySpan<byte> reserved = stackalloc byte[11 * sizeof(uint)];
        writer.Write(reserved); // dwReserved1
        
        // ---- DDS_PIXELFORMAT
        Console.WriteLine("Writing DDS pixelformat.");
        
        writer.Write(32); // dwSize

        const uint dxt1 = 0x31545844;
        const uint dxt3 = 0x33545844;
        const uint dxt5 = 0x35545844;
        const uint dx10 = 0x30315844;
        
        uint pfFlags = 0;
        uint fourCC = 0;
        if (format is Format.R8G8B8A8_UNorm)
            pfFlags |= 0x1 | 0x40;
        else
        {
            pfFlags |= 0x4;

            switch (format)
            {
                case Format.BC1_UNorm:
                case Format.BC1_UNorm_SRGB:
                    fourCC = dxt1;
                    break;
                
                case Format.BC2_UNorm:
                case Format.BC2_UNorm_SRGB:
                    fourCC = dxt3;
                    break;
                
                case Format.BC3_UNorm:
                case Format.BC3_UNorm_SRGB:
                    fourCC = dxt5;
                    break;
                
                case Format.BC4_UNorm:
                case Format.BC4_SNorm:
                case Format.BC5_UNorm:
                case Format.BC5_SNorm:
                case Format.BC6H_UF16:
                case Format.BC6H_SF16:
                case Format.BC7_UNorm:
                case Format.BC7_UNorm_SRGB:
                    fourCC = dx10;
                    break;
                
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        writer.Write(pfFlags);
        writer.Write(fourCC);
        
        writer.Write(32); // dwRGBBitCount
        
        writer.Write(0x000000FF); // dwRBitMask
        writer.Write(0x0000FF00); // dwGBitMask
        writer.Write(0x00FF0000); // dwBBitMask
        writer.Write(0xFF000000); // dwABitMask
        
        Console.WriteLine("DDS pixelformat written successfully.");
        // ----

        uint dwCaps = 0x1000;
        if (genMips)
            dwCaps |= 0x8 | 0x400000;
        
        writer.Write(dwCaps);
        
        writer.Write(0); // dwCaps2
        writer.Write(0); // dwCaps3
        writer.Write(0); // dwCaps4
        writer.Write(0); // dwReserved2
        
        Console.WriteLine("DDS header written successfully.");

        // DDS_HEADER_DXT10
        
        if (fourCC == dx10)
        {
            Console.WriteLine("Writing DXT10 header.");
            
            uint dxFormat = format switch
            {
                Format.R8G8B8A8_UNorm => 28,
                Format.BC1_UNorm => 71,
                Format.BC1_UNorm_SRGB => 72,
                Format.BC2_UNorm => 74,
                Format.BC2_UNorm_SRGB => 75,
                Format.BC3_UNorm => 77,
                Format.BC3_UNorm_SRGB => 78,
                Format.BC4_UNorm => 80,
                Format.BC4_SNorm => 81,
                Format.BC5_UNorm => 83,
                Format.BC5_SNorm => 84,
                Format.BC6H_UF16 => 95,
                Format.BC6H_SF16 => 96,
                Format.BC7_UNorm => 98,
                Format.BC7_UNorm_SRGB => 99,
                _ => throw new ArgumentOutOfRangeException()
            };
            
            writer.Write(dxFormat);
            writer.Write(3); // resourceDimension (Texture2D)
            writer.Write(0); // miscFlag
            writer.Write(1); // arraySize
            writer.Write(0x1); // miscFlags2
            
            Console.WriteLine("DXT10 header written successfully.");
        }
        
        // Write all texture data to file.
        Console.WriteLine("Begin writing data to file.");
        for (int i = 0; i < numMips; i++)
        {
            Console.WriteLine($"Writing texture level {i} to file.");
            writer.Write(texData[i]);
        }
        Console.WriteLine("Texture data written successfully.");
        
        Console.WriteLine("Writing DDS to file.");
        File.WriteAllBytes(Path.Combine(outDir, $"{name}.dds"), memStream.ToArray());
    }

    public override void Dispose()
    {
        base.Dispose();
        
        Console.WriteLine("Closing SDL.");
        _gl.Dispose();
        _sdl.GLDeleteContext(_glContext);
        _sdl.DestroyWindow(_window);
        _sdl.Quit();
        _sdl.Dispose();
    }
}