using System.Numerics;

namespace Euphoria.Engine.Entities;

public class Camera : Entity
{
    private float _fov;
    private float _near;
    private float _far;

    // TODO: CRAP!!! DO NOT DO IT LIKE THIS
    public Matrix4x4 ProjectionMatrix => Matrix4x4.CreatePerspectiveFieldOfView(_fov,
        App.Window.SizeInPixels.Width / (float) App.Window.SizeInPixels.Height, _near, _far);

    public Matrix4x4 ViewMatrix =>
        Matrix4x4.CreateLookAt(Transform.Position, Transform.Position + Transform.Forward, Transform.Up);

    public float FieldOfView
    {
        get => float.RadiansToDegrees(_fov);
        set => _fov = float.DegreesToRadians(value);
    }

    public float NearPlane
    {
        get => _near;
        set => _near = value;
    }

    public float FarPlane
    {
        get => _far;
        set => _far = value;
    }
    
    public Camera(string name, Transform transform, float fieldOfView, float near = 0.1f, float far = 1000f) 
        : base(name, transform)
    {
        _fov = float.DegreesToRadians(fieldOfView);
        _near = near;
        _far = far;
    }
}