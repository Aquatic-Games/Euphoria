using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Renderers;
using Euphoria.Render.Renderers.Structs;

namespace Tests.Render.Render3D;

public class PlaneTest : TestBase
{
    private Texture _texture;
    private Material _material;
    private Renderable _renderable;

    private Texture _debugTexture;

    private float _rotation;

    protected override void Initialize()
    {
        base.Initialize();
        
        Graphics.Renderer3D.BackgroundColor = Color.CornflowerBlue;
        Graphics.Renderer3D.Skybox = Graphics.CreateCubemap(
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\right.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\left.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\top.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\bottom.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\front.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\back.jpg"));

        _texture = new Texture("Content/awesomeface.png");
        _material = Graphics.Renderer3D.CreateMaterial(new MaterialDescription(_texture));

        Vertex[] vertices =
        [
            new Vertex(new Vector3(-0.5f, -0.5f, 0.0f), new Vector2(0, 1), new Color(1.0f, 0.0f, 0.0f), Vector3.Zero),
            new Vertex(new Vector3(-0.5f, +0.5f, 0.0f), new Vector2(0, 0), new Color(0.0f, 1.0f, 0.0f), Vector3.Zero),
            new Vertex(new Vector3(+0.5f, +0.5f, 0.0f), new Vector2(1, 0), new Color(0.0f, 0.0f, 1.0f), Vector3.Zero),
            new Vertex(new Vector3(+0.5f, -0.5f, 0.0f), new Vector2(1, 1), new Color(0.0f, 0.0f, 0.0f), Vector3.Zero)
        ];

        uint[] indices =
        [
            0, 1, 3,
            1, 2, 3
        ];

        Mesh mesh = new Mesh(vertices, indices);
        _renderable = Graphics.Renderer3D.CreateRenderable(mesh, _material);

        _debugTexture = new Texture("Content/DEBUG.png");
    }

    protected override void Update(float dt)
    {
        base.Update(dt);

        // TODO: MathHelper(?)
        Matrix4x4 projection =
            Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(45), 1280 / 720.0f, 0.1f, 100.0f);

        Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(0, 0, 3), Vector3.Zero, Vector3.UnitY);
        
        Renderer3D renderer = Graphics.Renderer3D;
        renderer.Camera = new CameraInfo(projection, view);

        _rotation += dt;
    }

    protected override void Draw()
    {
        base.Draw();

        for (int x = -1; x < 2; x++)
        {
            for (int y = -1; y < 2; y++)
            {
                Matrix4x4 world = Matrix4x4.CreateFromYawPitchRoll(_rotation, _rotation * 0.75f, _rotation * 1.1f) * Matrix4x4.CreateTranslation(x, y, 0);
                Graphics.Renderer3D.Draw(_renderable, world);
            }
        }
        
        /*Matrix4x4 world = Matrix4x4.CreateFromYawPitchRoll(_rotation, _rotation * 0.75f, _rotation * 1.1f) * Matrix4x4.CreateTranslation(0, 1, 0);
        Graphics.Renderer3D.Draw(_renderable, world);*/
        
        Graphics.TextureBatcher.Draw(_debugTexture, new Vector2(0, 0), Color.White);
    }

    public override void Dispose()
    {
        _debugTexture.Dispose();
        
        _renderable.Dispose();
        _material.Dispose();
        _texture.Dispose();
        
        base.Dispose();
    }

    public PlaneTest() : base("3D Renderer Plane Test") { }
}