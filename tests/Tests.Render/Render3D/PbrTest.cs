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

        _renderable = new Renderable(mesh, _material);
    }

    protected override void Update(float dt)
    {
        _value += dt;
    }

    protected override void Draw()
    {
        Renderer3D renderer = Graphics.Renderer3D;
        Size<int> size = Graphics.Size;
        renderer.Camera = new CameraInfo()
        {
            Projection = Matrix4x4.CreatePerspectiveFieldOfView(float.DegreesToRadians(75),
                size.Width / (float) size.Height, 0.1f, 100f),
            View = Matrix4x4.CreateLookAt(new Vector3(0, 1.5f, 2), Vector3.Zero, Vector3.UnitY)
        };

        Matrix4x4 world = Matrix4x4.CreateScale(5, 1, 5) * Matrix4x4.CreateFromAxisAngle(Vector3.UnitY, _value);
        //Matrix4x4 world = Matrix4x4.Identity;
        
        renderer.Draw(_renderable, world);


        TextureBatcher batcher = Graphics.TextureBatcher;
        (string, Texture texture)[] textures = renderer.GetDebugTextures();

        batcher.Draw(textures[0].texture, Vector2.Zero, Color.White);
    }

    public PbrTest() : base("PBR Test") { }
}