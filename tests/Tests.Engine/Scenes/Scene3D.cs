using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Entities.Components;
using Euphoria.Engine.Scenes;
using Euphoria.Render;
using Tests.Engine.Components;
using Plane = Euphoria.Render.Primitives.Plane;

namespace Tests.Engine.Scenes;

public class Scene3D : Scene
{
    private Texture _texture;
    private Material _material;
    
    public override void Initialize()
    {
        _texture = App.Graphics.CreateTexture(new Bitmap(@"C:\Users\ollie\Pictures\BAGELMIP.png"));

        // TODO: A bit like textures with the content manager, materials should be managed internally, so you never actually create or manage them yourself.
        MaterialDescription matDesc = new MaterialDescription(_texture);
        _material = App.Graphics.Renderer3D.CreateMaterial(matDesc);
        
        Plane plane = new Plane();
        Mesh mesh = new Mesh(plane.Vertices, plane.Indices);

        Entity entity = new Entity("Plane", new Transform(new Vector3(0, 0, -3)));
        entity.AddComponent(new MeshRenderer(mesh, _material));
        
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