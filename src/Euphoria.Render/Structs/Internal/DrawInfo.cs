using System.Numerics;
using System.Runtime.InteropServices;

namespace Euphoria.Render.Structs.Internal;

[StructLayout(LayoutKind.Sequential)]
internal readonly struct DrawInfo
{
    public readonly Matrix4x4 World;

    public readonly MaterialInfo Material;

    public DrawInfo(Matrix4x4 world, MaterialInfo material)
    {
        World = world;
        Material = material;
    }
}