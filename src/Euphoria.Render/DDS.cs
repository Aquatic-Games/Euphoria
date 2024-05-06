using System;
using System.IO;
using System.Runtime.CompilerServices;
using Euphoria.Render.Utilities;
using grabs.Graphics;
using u4.Math;

namespace Euphoria.Render;

public class DDS
{
    public const uint Magic = 0x20534444;
    
    public DDSHeader Header;

    public DDSHeaderDXT10? HeaderDxt10;

    public Bitmap[] ImageData;

    public DDS(DDSHeader header, DDSHeaderDXT10? headerDxt10, Bitmap[] data)
    {
        Header = header;
        HeaderDxt10 = headerDxt10;
        ImageData = data;
    }

    public static unsafe DDS FromFile(string path)
    {
        using FileStream stream = File.OpenRead(path);
        using BinaryReader reader = new BinaryReader(stream);

        if (reader.ReadUInt32() != Magic)
            throw new Exception("File is not a valid DDS file.");
        
        Span<byte> headerBytes = stackalloc byte[sizeof(DDSHeader)];
        if (reader.Read(headerBytes) != headerBytes.Length)
            throw new Exception("Could not read DDS header.");

        DDSHeader header = Unsafe.As<byte, DDSHeader>(ref headerBytes[0]);
        DDSHeaderDXT10? headerDxt10 = null;

        if (header.PixelFormat.FourCC == KnownFourCC.DX10)
        {
            Span<byte> headerDxt10Bytes = stackalloc byte[sizeof(DDSHeaderDXT10)];
            if (reader.Read(headerDxt10Bytes) != headerDxt10Bytes.Length)
                throw new Exception("Could not read DXT10 header.");

            headerDxt10 = Unsafe.As<byte, DDSHeaderDXT10>(ref headerDxt10Bytes[0]);
        }

        Format format;
        if ((header.PixelFormat.Flags & PixelFormatFlags.FourCC) == PixelFormatFlags.FourCC)
        {
            format = header.PixelFormat.FourCC switch
            {
                KnownFourCC.DXT1 => Format.BC1_UNorm,
                KnownFourCC.DXT3 => Format.BC2_UNorm,
                KnownFourCC.DXT5 => Format.BC3_UNorm,
                KnownFourCC.DX10 => Utils.DxgiFormatToGrabsFormat(headerDxt10.Value.DxgiFormat),
                _ => throw new NotSupportedException()
            };
        }
        else
        {
            uint rBitmask = header.PixelFormat.RBitMask;
            uint gBitmask = header.PixelFormat.GBitMask;
            uint bBitmask = header.PixelFormat.BBitMask;
            uint aBitmask = header.PixelFormat.ABitMask;
            uint rgbBitCount = header.PixelFormat.RGBBitCount;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            bool IsBitmask(uint r, uint g, uint b, uint a)
                => rBitmask == r && gBitmask == g && bBitmask == b && aBitmask == a;

            switch (rgbBitCount)
            {
                case 32:
                {
                    if (IsBitmask(0xFF, 0xFF00, 0xFF0000, 0xFF000000))
                        format = Format.R8G8B8A8_UNorm;
                    else
                        throw new NotImplementedException();

                    break;
                }
                
                default:
                    throw new NotImplementedException();
            }
        }

        bool isCompressed = format.IsCompressed();

        uint totalSize;
        if (isCompressed)
            totalSize = header.PitchOrLinearSize;
        else
            totalSize = header.PitchOrLinearSize * header.Height;

        uint numMips = header.MipMapCount == 0 ? 1 : header.MipMapCount;
        
        Bitmap[] bitmaps = new Bitmap[numMips];

        int width = (int) header.Width;
        int height = (int) header.Height;
        
        for (int i = 0; i < numMips; i++)
        {
            bitmaps[i] = new Bitmap(reader.ReadBytes((int) totalSize), new Size<int>(width, height), format);

            width = int.Max(1, width / 2);
            height = int.Max(1, height / 2);

            totalSize /= 4;
        }

        return new DDS(header, headerDxt10, bitmaps);
    }
    
    public unsafe struct DDSHeader
    {
        public uint Size;
        public Flags Flags;
        public uint Height;
        public uint Width;
        public uint PitchOrLinearSize;
        public uint Depth;
        public uint MipMapCount;
        public fixed uint Reserved1[11];
        public PixelFormat PixelFormat;
        public Caps Caps;
        public Caps2 Caps2;
        public uint Caps3;
        public uint Caps4;
        public uint Reserved2;
    }

    public struct PixelFormat
    {
        public uint Size;
        public PixelFormatFlags Flags;
        public KnownFourCC FourCC;
        public uint RGBBitCount;
        public uint RBitMask;
        public uint GBitMask;
        public uint BBitMask;
        public uint ABitMask;
    }

    public struct DDSHeaderDXT10
    {
        public DxgiFormat DxgiFormat;
        public ResourceDimension ResourceDimension;
        public MiscFlags MiscFlag;
        public uint ArraySize;
        public MiscFlags2 MiscFlags2;
    }

    [Flags]
    public enum Flags : uint
    {
        None = 0,
        
        Caps = 0x1,
        Height = 0x2,
        Width = 0x4,
        Pitch = 0x8,
        PixelFormat = 0x1000,
        MipMapCount = 0x20000,
        LinearSize = 0x80000,
        Depth = 0x800000
    }

    [Flags]
    public enum Caps : uint
    {
        None = 0,
        
        Complex = 0x8,
        Texture = 0x1000,
        MipMap = 0x400000
    }

    [Flags]
    public enum Caps2 : uint
    {
        None = 0,
        
        Cubemap = 0x200,
        CubemapPositiveX = 0x400,
        CubemapNegativeX = 0x800,
        CubemapPositiveY = 0x1000,
        CubemapNegativeY = 0x2000,
        CubemapPositiveZ = 0x4000,
        CubemapNegativeZ = 0x8000,
        Volume = 0x200000
    }

    [Flags]
    public enum PixelFormatFlags : uint
    {
        None = 0,
        
        AlphaPixels = 0x1,
        Alpha = 0x2,
        FourCC = 0x4,
        RGB = 0x40,
        YUV = 0x200,
        Luminance = 0x20000
    }

    public enum KnownFourCC : uint
    {
        DXT1 = 0x31545844,
        DXT2 = 0x32545844,
        DXT3 = 0x33545844,
        DXT4 = 0x34545844,
        DXT5 = 0x35545844,
        DX10 = 0x30315844
    }

    public enum ResourceDimension : uint
    {
        Texture1D = 2,
        Texture2D = 3,
        Texture3D = 4
    }

    [Flags]
    public enum MiscFlags : uint
    {
        TextureCube = 0x4
    }

    [Flags]
    public enum MiscFlags2 : uint
    {
        AlphaModeUnknown = 0x0,
        AlphaModeStraight = 0x1,
        AlphaModePremultiplied = 0x2,
        AlphaModeOpaque = 0x3,
        AlphaModeCustom = 0x4
    }
}