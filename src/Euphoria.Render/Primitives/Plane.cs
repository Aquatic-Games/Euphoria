using System.Numerics;
using Euphoria.Math;

namespace Euphoria.Render.Primitives;

public class Plane : IPrimitive
{
    public Vertex[] Vertices { get; }
    
    public uint[] Indices { get; }

    public Plane()
    {
        Vertices =
        [
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 1), Color.White, Vector3.UnitZ),
            new Vertex(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 0), Color.White, Vector3.UnitZ),
            new Vertex(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 0), Color.White, Vector3.UnitZ),
            new Vertex(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 1), Color.White, Vector3.UnitZ)
        ];

        Indices =
        [
            0, 1, 3,
            1, 2, 3
        ];
    }
}