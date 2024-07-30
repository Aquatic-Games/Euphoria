using System.Numerics;

namespace Euphoria.Render.Renderers.Structs;

public readonly struct CameraInfo
{
    public readonly Matrix4x4 Projection;
    public readonly Matrix4x4 View;
    public readonly Vector4 Position;

    public CameraInfo(Matrix4x4 projection, Matrix4x4 view, Vector3 position)
    {
        Projection = projection;
        View = view;
        Position = new Vector4(position, 0);
    }
}