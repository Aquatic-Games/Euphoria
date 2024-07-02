using Euphoria.Render;

namespace Euphoria.Engine.Entities.Components;

public class MeshRenderer : Component
{
    private Renderable _renderable;

    public Renderable Renderable => _renderable;

    public MeshRenderer(Mesh mesh, Material material)
    {
        _renderable = App.Graphics.Renderer3D.CreateRenderable(mesh, material);
    }

    public MeshRenderer(Renderable renderable)
    {
        _renderable = renderable;
    }

    public override void Draw()
    {
        App.Graphics.Renderer3D.Draw(_renderable, Transform.WorldMatrix);
    }

    public override void Dispose()
    {
        _renderable.Dispose();
    }
}