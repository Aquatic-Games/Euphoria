using System;
using System.Numerics;

namespace Euphoria.Render;

public class Mesh
{
    public Vertex[] Vertices;
    public uint[] Indices;

    // TODO: Should a mesh have a material? Probably.
    public Mesh(Vertex[] vertices, uint[] indices)
    {
        Vertices = vertices;
        Indices = indices;
    }

    public void CalculateTangents()
    {
        for (int i = 0; i < Indices.Length; i += 3)
        {
            ref Vertex v1 = ref Vertices[Indices[i + 0]];
            ref Vertex v2 = ref Vertices[Indices[i + 1]];
            ref Vertex v3 = ref Vertices[Indices[i + 2]];

            Vector3 edge1 = v2.Position - v1.Position;
            Vector3 edge2 = v3.Position - v1.Position;
            Vector2 deltaUv1 = v2.TexCoord - v1.TexCoord;
            Vector2 deltaUv2 = v3.TexCoord - v1.TexCoord;

            float f = 1.0f / (deltaUv1.X * deltaUv2.Y - deltaUv2.X * deltaUv1.Y);

            Vector3 tangent = new Vector3()
            {
                X = f * (deltaUv2.Y * edge1.X - deltaUv1.Y * edge2.X),
                Y = f * (deltaUv2.Y * edge1.Y - deltaUv1.Y * edge2.Y),
                Z = f * (deltaUv2.Y * edge1.Z - deltaUv1.Y * edge2.Z)
            };

            v1.Tangent = tangent;
            v2.Tangent = tangent;
            v3.Tangent = tangent;
        }
    }
}