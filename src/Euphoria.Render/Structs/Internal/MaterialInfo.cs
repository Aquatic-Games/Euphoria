using System.Runtime.InteropServices;
using Euphoria.Math;

namespace Euphoria.Render.Structs.Internal;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MaterialInfo
{
    public readonly Color AlbedoColor;

    public MaterialInfo(Color albedoColor)
    {
        AlbedoColor = albedoColor;
    }
}