using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities.Components;
using Euphoria.Math;
using Euphoria.Render;

namespace Tests.Engine.Components;

public class SpriteComponent : Component
{
    public Texture Texture;

    public SpriteComponent(Texture texture)
    {
        Texture = texture;
    }

    public override void Draw()
    {
        Vector3 position = Transform.Position;
        
        Graphics.TextureBatcher.Draw(Texture, new Vector2(position.X, position.Y), Color.White);
    }
}