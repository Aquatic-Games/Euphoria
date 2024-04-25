using System.Numerics;

namespace Euphoria.Render.Renderers;

public struct Sprite
{
    public Texture Texture;
    public Matrix3x2 World;
    public float ZIndex;

    public Sprite(Texture texture, Matrix3x2 world, float zIndex)
    {
        Texture = texture;
        World = world;
        ZIndex = zIndex;
    }

    public Sprite(Texture texture, Vector3 position)
    {
        Texture = texture;
        World = Matrix3x2.CreateTranslation(position.X, position.Y);
        ZIndex = position.Z;
    }

    public Sprite(Texture texture, Vector3 position, float rotation)
    {
        Texture = texture;
        World = Matrix3x2.CreateRotation(rotation) *
                Matrix3x2.CreateTranslation(position.X, position.Y);
        ZIndex = position.Z;
    }
}