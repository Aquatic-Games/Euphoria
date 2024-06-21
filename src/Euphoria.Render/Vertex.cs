using System.Numerics;
using System.Runtime.InteropServices;
using u4.Math;

namespace Euphoria.Render;

[StructLayout(LayoutKind.Sequential)]
public struct Vertex
{
    public Vector3 Position;
    public Vector2 TexCoord;
    public Color   Color;
    public Vector3 Normal;

    public Vertex(Vector3 position, Vector2 texCoord, Color color, Vector3 normal)
    {
        Position = position;
        TexCoord = texCoord;
        Color = color;
        Normal = normal;
    }

    public const uint SizeInBytes = 48;
}