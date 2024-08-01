using System.Numerics;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Entities.Components;
using Euphoria.Engine.Scenes;
using Euphoria.Physics;
using Euphoria.Physics.Shapes;
using Euphoria.Render;
using Euphoria.Render.Primitives;
using Tests.Engine.Components;

namespace Tests.Engine.Scenes;

public class PhysicsScene : Scene
{
    public override void Initialize()
    {
        base.Initialize();

        Camera.Transform.Position.Z = 3;
        Camera.AddComponent(new CameraMove());

        Cube cube = new Cube();
        Material material = new Material(new MaterialDescription(Texture.White));
        
        Entity staticCube = new Entity("StaticCube",
            new Transform(new Vector3(0, -2, 0), Quaternion.Identity, new Vector3(10, 1, 10), Vector3.Zero));
        staticCube.AddComponent(new MeshRenderer(new Mesh(cube.Vertices, cube.Indices), material));
        staticCube.AddComponent(new PhysicsComponent(new Box(10, 1, 10), 0));
        
        AddEntity(staticCube);

        for (int i = 0; i < 100; i++)
        {
            Entity dynamicCube = new Entity($"DynamicCube{i}", new Transform(new Vector3(0, 15 + i, 0)));
            dynamicCube.AddComponent(new MeshRenderer(new Mesh(cube.Vertices, cube.Indices), material));
            dynamicCube.AddComponent(new PhysicsComponent(new Box(1, 1, 1), 1));
            
            AddEntity(dynamicCube);
        }
    }
}