using System.Runtime.InteropServices;
using Euphoria.Math;

namespace Euphoria.Render.Structs.Internal;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct MaterialInfo
{
    public readonly Color AlbedoColor;

    public readonly float MetallicColor;

    public readonly float RoughnessColor;

    private readonly double _padding;

    public MaterialInfo(Color albedoColor, float metallicColor, float roughnessColor)
    {
        AlbedoColor = albedoColor;
        MetallicColor = metallicColor;
        RoughnessColor = roughnessColor;
    }
}