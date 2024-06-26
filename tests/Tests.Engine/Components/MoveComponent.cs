using Euphoria.Engine.Entities.Components;

namespace Tests.Engine.Components;

public class MoveComponent : Component
{
    public override void Update(float dt)
    {
        base.Update(dt);

        // TODO: Probably should add input...
        Transform.Position.X += 50 * dt;
    }
}