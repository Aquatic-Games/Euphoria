using System.Numerics;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using Euphoria.Render.Renderers.Structs;
using u4.Math;

namespace Tests.Render.Render3D;

public class PlaneTest : TestBase
{
    private Texture _texture;
    private Material _material;
    private Renderable _renderable;

    private float _rotation;

    protected override void Initialize()
    {
        base.Initialize();

        _texture = Graphics.CreateTexture(new Bitmap("Content/awesomeface.png"));
        _material = Graphics.Renderer3D.CreateMaterial(new MaterialDescription(_texture));

        Vertex[] vertices =
        [
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 0), Color.White, Vector3.Zero),
            new Vertex(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 1), Color.White, Vector3.Zero),
            new Vertex(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 1), Color.White, Vector3.Zero),
            new Vertex(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 0), Color.White, Vector3.Zero)
        ];

        uint[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        Mesh mesh = new Mesh(vertices, indices);
        _renderable = Graphics.Renderer3D.CreateRenderable(mesh, _material);
    }

    protected override void Update(float dt)
    {
        base.Update(dt);

        // TODO: MathHelper(?)
        Matrix4x4 projection =
            Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(45), 1280 / 720.0f, 0.1f, 100.0f);

        Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(0, 0, -3), Vector3.Zero, Vector3.UnitY);
        
        Renderer3D renderer = Graphics.Renderer3D;
        renderer.Camera = new CameraInfo(projection, view);

        _rotation += dt;
    }

    protected override void Draw()
    {
        base.Draw();

        Matrix4x4 world = Matrix4x4.CreateFromYawPitchRoll(_rotation, _rotation * 0.75f, _rotation * 1.1f);
        
        Graphics.Renderer3D.Draw(_renderable, world);
    }

    public PlaneTest() : base("3D Renderer Cube Test") { }
}