using System.Numerics;

namespace Euphoria.Render.Renderers.Structs;

public readonly struct TransformedRenderable
{
    public readonly Renderable Renderable;

    public readonly Matrix4x4 Transform;

    public TransformedRenderable(Renderable renderable, Matrix4x4 transform)
    {
        Renderable = renderable;
        Transform = transform;
    }
}