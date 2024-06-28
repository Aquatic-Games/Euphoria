using Euphoria.Render;

namespace Euphoria.Engine.Entities.Components;

public class MeshRenderer : Component
{
    private Renderable _renderable;

    public MeshRenderer(Mesh mesh, Material material)
    {
        _renderable = App.Graphics.Renderer3D.CreateRenderable(mesh, material);
    }

    public override void Draw()
    {
        App.Graphics.Renderer3D.Draw(_renderable, Transform.WorldMatrix);
    }
}