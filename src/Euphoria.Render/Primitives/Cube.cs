using System.Numerics;
using Euphoria.Math;

namespace Euphoria.Render.Primitives;

public class Cube : IPrimitive
{
    public Vertex[] Vertices { get; }

    public uint[] Indices { get; }

    public Cube()
    {
        Vertices = new Vertex[]
        {
            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 0), Color.White, new Vector3(0, 1, 0), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(1, 0), Color.White, new Vector3(0, 1, 0), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1, 1), Color.White, new Vector3(0, 1, 0), Vector3.UnitX),
            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(0, 1), Color.White, new Vector3(0, 1, 0), Vector3.UnitX),

            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(0, 0), Color.White, new Vector3(0, -1, 0), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1, 0), Color.White, new Vector3(0, -1, 0), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(1, 1), Color.White, new Vector3(0, -1, 0), Vector3.UnitX),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1), Color.White, new Vector3(0, -1, 0), Vector3.UnitX),

            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(0, 0), Color.White, new Vector3(-1, 0, 0), Vector3.UnitZ),
            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(1, 0), Color.White, new Vector3(-1, 0, 0), Vector3.UnitZ),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(1, 1), Color.White, new Vector3(-1, 0, 0), Vector3.UnitZ),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(0, 1), Color.White, new Vector3(-1, 0, 0), Vector3.UnitZ),

            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(0, 0), Color.White, new Vector3(1, 0, 0), -Vector3.UnitZ),
            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(1, 0), Color.White, new Vector3(1, 0, 0), -Vector3.UnitZ),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(1, 1), Color.White, new Vector3(1, 0, 0), -Vector3.UnitZ),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(0, 1), Color.White, new Vector3(1, 0, 0), -Vector3.UnitZ),

            new Vertex(new Vector3(0.5f, 0.5f, -0.5f), new Vector2(0, 0), Color.White, new Vector3(0, 0, -1), -Vector3.UnitX),
            new Vertex(new Vector3(-0.5f, 0.5f, -0.5f), new Vector2(1, 0), Color.White, new Vector3(0, 0, -1), -Vector3.UnitX),
            new Vertex(new Vector3(-0.5f, -0.5f, -0.5f), new Vector2(1, 1), Color.White, new Vector3(0, 0, -1), -Vector3.UnitX),
            new Vertex(new Vector3(0.5f, -0.5f, -0.5f), new Vector2(0, 1), Color.White, new Vector3(0, 0, -1), -Vector3.UnitX),

            new Vertex(new Vector3(-0.5f, 0.5f, 0.5f), new Vector2(0, 0), Color.White, new Vector3(0, 0, 1), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, 0.5f, 0.5f), new Vector2(1, 0), Color.White, new Vector3(0, 0, 1), Vector3.UnitX),
            new Vertex(new Vector3(0.5f, -0.5f, 0.5f), new Vector2(1, 1), Color.White, new Vector3(0, 0, 1), Vector3.UnitX),
            new Vertex(new Vector3(-0.5f, -0.5f, 0.5f), new Vector2(0, 1), Color.White, new Vector3(0, 0, 1), Vector3.UnitX)
        };

        Indices = new uint[]
        {
            0, 1, 2, 0, 2, 3,
            4, 5, 6, 4, 6, 7,
            8, 9, 10, 8, 10, 11,
            12, 13, 14, 12, 14, 15,
            16, 17, 18, 16, 18, 19,
            20, 21, 22, 20, 22, 23
        };
    }
}