using System.Runtime.CompilerServices;
using Crimson.Core;
using Crimson.Graphics.Utils;
using Crimson.Math;
using piko.SDL3;

namespace Crimson.Graphics;

internal sealed unsafe class RendererContext : IDisposable
{
    /// <summary>
    /// The initial size of the transfer buffer (32MiB)
    /// </summary>
    private const uint InitialTransferBufferSize = 32 * 1024 * 1024;

    private SDL.GPUTransferBuffer _transferBuffer;
    private uint _transferBufferSize;
    private uint _transferBufferOffset;

    public readonly SDL.Window Window;
    public readonly SDL.GPUDevice Device;

    public RendererContext(SDL.Window window)
    {
        Window = window;

        uint props = SDL.CreateProperties();
        // always enable vulkan as a fallback and for linux
        SDL.SetBooleanProperty(props, SDL.Prop.GpuDeviceCreateShadersSpirvBoolean, true);

        // enable d3d12 on windows
        if (OperatingSystem.IsWindows())
            SDL.SetBooleanProperty(props, SDL.Prop.GpuDeviceCreateShadersDxilBoolean, true);

        // enable metal on macos
        if (OperatingSystem.IsMacOS())
            SDL.SetBooleanProperty(props, SDL.Prop.GpuDeviceCreateShadersMslBoolean, true);

#if DEBUG
        SDL.SetBooleanProperty(props, SDL.Prop.GpuDeviceCreateDebugmodeBoolean, true);
        SDL.SetBooleanProperty(props, SDL.Prop.GpuDeviceCreateVerboseBoolean, true);
#endif

        Logger.Trace("Creating device.");
        Device = SDL.CreateGPUDeviceWithProperties(props).Check("Create device");
        SDL.DestroyProperties(props);

        uint deviceProps = SDL.GetGPUDeviceProperties(Device);
        Logger.Info($"Backend: {SDL.GetGPUDeviceDriver(Device)}");
        Logger.Info($"Device: {SDL.GetStringProperty(deviceProps, SDL.Prop.GpuDeviceNameString, "unknown")}");
        Logger.Info($"Driver: {SDL.GetStringProperty(deviceProps, SDL.Prop.GpuDeviceDriverInfoString, "unknown")}");
        SDL.DestroyProperties(deviceProps);

        Logger.Trace("Claiming window for device.");
        SDL.ClaimWindowForGPUDevice(Device, Window).Check("Claim window for device");

        _transferBufferSize = InitialTransferBufferSize;
        _transferBufferOffset = 0;
        _transferBuffer = CreateTransferBuffer(SDL.GPUTransferBufferUsage.Upload, _transferBufferSize);
    }

    public SDL.GPUTransferBuffer CreateTransferBuffer(SDL.GPUTransferBufferUsage usage, uint size)
    {
        SDL.GPUTransferBufferCreateInfo transferBufferInfo = new()
        {
            Usage = usage,
            Size = size
        };

        Logger.Trace($"Creating {size / 1024}KiB transfer buffer.");
        return SDL.CreateGPUTransferBuffer(Device, &transferBufferInfo).Check("Create transfer buffer");
    }

    /// <summary>
    /// Get a transfer buffer to upload data to.
    /// </summary>
    /// <param name="size">The size of the data to upload.</param>
    /// <param name="offset">The offset into the buffer that the data should be written to.</param>
    /// <param name="shouldCycle">If <see langword="true"/>, the buffer should be cycled when mapping it.</param>
    public SDL.GPUTransferBuffer GetUploadBuffer(uint size, out uint offset, out bool shouldCycle)
    {
        if (size >= _transferBufferSize)
        {
            Logger.Debug($"Requested size ({size / 1024}KiB) is larger than the transfer buffer ({_transferBufferSize / 1024}KiB). The buffer will be resized.");
            // ensure the buffer is large enough to fit the data, then round it up to the next power of 2 for extra room.
            _transferBufferSize = BitUtils.RoundToNextPowerOf2(_transferBufferSize);
            SDL.ReleaseGPUTransferBuffer(Device, _transferBuffer);
            _transferBuffer = CreateTransferBuffer(SDL.GPUTransferBufferUsage.Upload, _transferBufferSize);
        }

        // if the buffer at the current offset cannot hold the data,
        // reset the offset to 0 and signal that it needs to be cycled.
        shouldCycle = false;
        if (size + _transferBufferOffset >= _transferBufferSize)
        {
            shouldCycle = true;
            _transferBufferOffset = 0;
        }

        offset = _transferBufferOffset;
        _transferBufferOffset += size;

        return _transferBuffer;
    }

    // todo replace x, y with Vec2<uint>
    public void CopyDataToTexture(SDL.GPUTexture texture, nint data, uint x, uint y, Size<uint> size, PixelFormat format)
    {
        uint dataSize = size.Width * size.Height * format.BytesPerPixel;

        Logger.Trace($"Uploading {dataSize} bytes of data to texture {texture.Handle}");
        SDL.GPUTransferBuffer transBuffer = GetUploadBuffer(dataSize, out uint offset, out bool cycle);

        nint mapped = SDL.MapGPUTransferBuffer(Device, transBuffer, cycle).Check("Map buffer");
        Unsafe.CopyBlock((byte*) mapped + offset, (void*) data, dataSize);
        SDL.UnmapGPUTransferBuffer(Device, transBuffer);

        SDL.GPUCommandBuffer cb = SDL.AcquireGPUCommandBuffer(Device).Check("Acquire command buffer");
        SDL.GPUCopyPass pass = SDL.BeginGPUCopyPass(cb).Check("Begin copy pass");

        SDL.GPUTextureTransferInfo src = new()
        {
            TransferBuffer = transBuffer,
            Offset = offset,
            PixelsPerRow = size.Width,
            RowsPerLayer = size.Height
        };

        SDL.GPUTextureRegion dest = new()
        {
            Texture = texture,
            X = x,
            Y = y,
            Z = 0,
            W = size.Width,
            H = size.Height,
            D = 1,
            Layer = 0,
            MipLevel = 0
        };

        SDL.UploadToGPUTexture(pass, &src, &dest, false);

        SDL.EndGPUCopyPass(pass);
        SDL.SubmitGPUCommandBuffer(cb);
    }

    public void Dispose()
    {
        SDL.ReleaseGPUTransferBuffer(Device, _transferBuffer);
        SDL.ReleaseWindowFromGPUDevice(Device, Window);
        SDL.DestroyGPUDevice(Device);
    }
}