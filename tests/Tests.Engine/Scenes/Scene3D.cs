using System;
using System.Numerics;
using Euphoria.Engine;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Entities.Components;
using Euphoria.Engine.InputSystem;
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
            new Bitmap($"{TestApp.FileBase}/Pictures/skybox/right.jpg"),
            new Bitmap($"{TestApp.FileBase}/Pictures/skybox/left.jpg"),
            new Bitmap($"{TestApp.FileBase}/Pictures/skybox/top.jpg"),
            new Bitmap($"{TestApp.FileBase}/Pictures/skybox/bottom.jpg"),
            new Bitmap($"{TestApp.FileBase}/Pictures/skybox/front.jpg"),
            new Bitmap($"{TestApp.FileBase}/Pictures/skybox/back.jpg"));
        
        _texture = new Texture($"{TestApp.FileBase}/Pictures/BAGELMIP.png");

        // TODO: A bit like textures with the content manager, materials should be managed internally, so you never actually create or manage them yourself.
        MaterialDescription matDesc = new MaterialDescription(new Texture($"{TestApp.FileBase}/Downloads/metalgrid2-dx/metalgrid2_basecolor.png"))
        {
            Normal = new Texture($"{TestApp.FileBase}/Downloads/metalgrid2-dx/metalgrid2_normal-dx.png"),
            Metallic = new Texture($"{TestApp.FileBase}/Downloads/metalgrid2-dx/metalgrid2_metallic.png"),
            Roughness = new Texture($"{TestApp.FileBase}/Downloads/metalgrid2-dx/metalgrid2_roughness.png"),
            Occlusion = new Texture($"{TestApp.FileBase}/Downloads/metalgrid2-dx/metalgrid2_AO.png")
        };
        _material = new Material(matDesc);
        
        Plane plane = new Plane();
        Mesh mesh = new Mesh(plane.Vertices, plane.Indices);

        Renderable renderable = new Renderable(32, 32, _material, UpdateFlags.Dynamic);
        renderable.Update(mesh);

        Cube cube = new Cube();
        Mesh cubeMesh = new Mesh(cube.Vertices, cube.Indices);
        //cubeMesh.CalculateTangents();
        
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