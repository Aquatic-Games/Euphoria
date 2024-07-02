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
        App.Graphics.Renderer3D.Skybox = App.Graphics.CreateCubemap(
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\right.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\left.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\top.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\bottom.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\front.jpg"),
            new Bitmap(@"C:\Users\ollie\Pictures\skybox\back.jpg"));
        
        _texture = App.Graphics.CreateTexture(new Bitmap(@"C:\Users\ollie\Pictures\BAGELMIP.png"));

        // TODO: A bit like textures with the content manager, materials should be managed internally, so you never actually create or manage them yourself.
        MaterialDescription matDesc = new MaterialDescription(_texture);
        _material = App.Graphics.Renderer3D.CreateMaterial(matDesc);
        
        Plane plane = new Plane();
        Mesh mesh = new Mesh(plane.Vertices, plane.Indices);

        Renderable renderable = App.Graphics.Renderer3D.CreateRenderable(mesh, _material, UpdateFlags.Dynamic);
        renderable.Update(mesh);

        Cube cube = new Cube();
        Mesh cubeMesh = new Mesh(cube.Vertices, cube.Indices);
        
        //renderable.Update(cubeMesh);

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