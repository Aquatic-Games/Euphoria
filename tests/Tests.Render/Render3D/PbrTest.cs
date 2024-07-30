using System.Numerics;
using Euphoria.Math;
using Euphoria.Render;
using Euphoria.Render.Primitives;
using Euphoria.Render.Renderers;
using Euphoria.Render.Renderers.Structs;
using grabs.Graphics;
using Texture = Euphoria.Render.Texture;

namespace Tests.Render.Render3D;

public class PbrTest : TestBase
{
    private Material _material;
    private Renderable _renderable;

    private float _value;
    
    protected override void Initialize()
    {
        Graphics.Renderer3D.BackgroundColor = Color.CornflowerBlue;
        
        Graphics.Renderer3D.Skybox = new Cubemap(
            new Bitmap($"{TestBase.FileBase}/Pictures/skybox/right.jpg"),
            new Bitmap($"{TestBase.FileBase}/Pictures/skybox/left.jpg"),
            new Bitmap($"{TestBase.FileBase}/Pictures/skybox/top.jpg"),
            new Bitmap($"{TestBase.FileBase}/Pictures/skybox/bottom.jpg"),
            new Bitmap($"{TestBase.FileBase}/Pictures/skybox/front.jpg"),
            new Bitmap($"{TestBase.FileBase}/Pictures/skybox/back.jpg"));

        Cube cube = new Cube();
        Mesh mesh = new Mesh(cube.Vertices, cube.Indices);
        
        SamplerDescription sampler = SamplerDescription.Anisotropic16x;

        Texture albedo = new Texture("Content/metalgrid1-dx-1/metalgrid1_basecolor.png", sampler);
        Texture normal = new Texture("Content/metalgrid1-dx-1/metalgrid1_normal-dx.png", sampler);
        Texture metallic = new Texture("Content/metalgrid1-dx-1/metalgrid1_metallic.png", sampler);
        Texture roughness = new Texture("Content/metalgrid1-dx-1/metalgrid1_roughness.png", sampler);
        Texture occlusion = new Texture("Content/metalgrid1-dx-1/metalgrid1_AO.png", sampler);

        _material = new Material(new MaterialDescription(albedo)
        {
            Normal = normal,
            Metallic = metallic,
            Roughness = roughness,
            Occlusion = occlusion
        });

        //_material = new Material(new MaterialDescription(Texture.White));

        _renderable = new Renderable(mesh, _material);
    }

    protected override void Update(float dt)
    {
        _value += dt;

        //_material.AlbedoColor = new Color(Color.White, (float.Sin(_value) + 1) * 0.5f);
        _material.AlbedoColor = new Color(Color.White, 0.5f);
    }

    protected override void Draw()
    {
        Renderer3D renderer = Graphics.Renderer3D;
        Size<int> size = Graphics.Size;

        Matrix4x4 projection = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(75),
            size.Width / (float) size.Height, 0.1f, 100f);
        Matrix4x4 view = Matrix4x4.CreateLookAt(new Vector3(0, 1.5f, 2), Vector3.Zero, Vector3.UnitY);

        renderer.Camera = new CameraInfo(projection, view, new Vector3(0, 1.5f, 2));

        Matrix4x4 world = Matrix4x4.CreateScale(5, 1, 5) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, _value * 0.3f);
        //Matrix4x4 world = Matrix4x4.CreateFromYawPitchRoll(_value * 0.75f, _value, _value * 1.3f);
        //Matrix4x4 world = Matrix4x4.Identity;
        
        renderer.Draw(_renderable, world);


        TextureBatcher batcher = Graphics.TextureBatcher;
        (string, Texture texture)[] textures = renderer.GetDebugTextures();
        
        //batcher.Draw(textures[3].texture, Vector2.Zero, Color.White);
    }

    public PbrTest() : base("PBR Test") { }
}