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

        Cube cube = new Cube();
        Material material = new Material(new MaterialDescription(Texture.White));

        Entity dynamicCube = new Entity("DynamicCube");
        dynamicCube.AddComponent(new MeshRenderer(new Mesh(cube.Vertices, cube.Indices), material));
        dynamicCube.AddComponent(new PhysicsComponent(new Box(1, 1, 1), 1));
        
        AddEntity(dynamicCube);
    }
}