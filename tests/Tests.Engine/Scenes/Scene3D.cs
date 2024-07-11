using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Entities.Components;
using Euphoria.Engine.Scenes;
using Euphoria.Render;
using Euphoria.Render.Primitives;
using Tests.Engine.Components;
using Plane = Euphoria.Render.Primitives.Plane;

namespace Tests.Engine.Scenes;

public class Scene3D : Scene
{
    private Texture _texture;
    private Material _material;
    
    public override void Initialize()
    {
        Graphics.Renderer3D.Skybox = new Cubemap(
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\right.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\left.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\top.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\bottom.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\front.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\back.jpg"));
        
        _texture = new Texture(@"C:\Users\ollie\Pictures\BAGELMIP.png");

        // TODO: A bit like textures with the content manager, materials should be managed internally, so you never actually create or manage them yourself.
        MaterialDescription matDesc = new MaterialDescription(
            new Texture(@"C:\Users\ollie\Downloads\metalgrid2-dx\metalgrid2_basecolor.png"),
            new Texture(@"C:\Users\ollie\Downloads\metalgrid2-dx\metalgrid2_normal-dx.png"));
        _material = new Material(matDesc);
        
        Plane plane = new Plane();
        Mesh mesh = new Mesh(plane.Vertices, plane.Indices);

        Renderable renderable = new Renderable(32, 32, _material, UpdateFlags.Dynamic);
        renderable.Update(mesh);

        Cube cube = new Cube();
        Mesh cubeMesh = new Mesh(cube.Vertices, cube.Indices);
        //cubeMesh.CalculateTangents();

        foreach (Vertex vertex in cubeMesh.Vertices)
        {
            Vector3 pos = vertex.Position;
            Vector2 uv = vertex.TexCoord;
            Vector3 nm = vertex.Normal;
            Vector3 tan = vertex.Tangent;
            
            Console.WriteLine($"new Vertex(new Vector3({pos.X}, {pos.Y}, {pos.Z}), new Vector2({uv.X}, {uv.Y}), Color.White, new Vector3({nm.X}, {nm.Y}, {nm.Z}), new Vector3({tan.X}, {tan.Y}, {tan.Z})),");
        }
        
        renderable.Update(cubeMesh);

        Entity entity = new Entity("Plane", new Transform(new Vector3(0, 0, -3)));
        entity.AddComponent(new MeshRenderer(renderable));
        
        AddEntity(entity);
        
        Camera.AddComponent(new CameraMove());
        
        base.Initialize();
    }

    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.IsKeyPressed(Key.O))
            SceneManager.LoadAndSwitchScene(new TestScene());
    }

    public override void Dispose()
    {
        _material.Dispose();
        _texture.Dispose();
    }
}