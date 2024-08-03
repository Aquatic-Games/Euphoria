using System;
using Euphoria.Engine.Entities.Components;
using Euphoria.Math;

namespace Tests.Engine.Components;

public class HighlightComponent : Component
{
    private Color _color;
    
    public bool Highlight;

    public override void Initialize()
    {
        MeshRenderer mesh = Entity.GetComponent<MeshRenderer>();
        _color = mesh.Renderable.Material.AlbedoColor;
    }

    public override void Update(float dt)
    {
        Highlight = false;
    }

    public override void Draw()
    {
        MeshRenderer mesh = Entity.GetComponent<MeshRenderer>();

        if (!Highlight)
        {
            mesh.Renderable.Material.AlbedoColor = _color;
            return;
        }
        
        mesh.Renderable.Material.AlbedoColor = Color.Green;
    }
}