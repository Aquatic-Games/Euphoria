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

        const bool interpolation = true;
        
        Entity staticCube = new Entity("StaticCube",
            new Transform(new Vector3(0, -2, 0), Quaternion.Identity, new Vector3(20, 1, 20), Vector3.Zero));
        staticCube.AddComponent(new MeshRenderer(new Mesh(cube.Vertices, cube.Indices), material));
        staticCube.AddComponent(new Rigidbody(new Box(20, 1, 20), 0, interpolation));
        
        AddEntity(staticCube);

        for (int i = 0; i < 1000; i++)
        {
            Entity dynamicCube = new Entity($"DynamicCube{i}", new Transform(new Vector3((i % 20) - 10, 15 + i, (i % 20) - 10)));
            dynamicCube.AddComponent(new MeshRenderer(new Mesh(cube.Vertices, cube.Indices), material));
            dynamicCube.AddComponent(new Rigidbody(new Box(1, 1, 1), 1, interpolation));
            
            AddEntity(dynamicCube);
        }
    }
}