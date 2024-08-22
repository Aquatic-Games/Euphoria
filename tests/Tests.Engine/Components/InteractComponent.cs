using System.Numerics;
using Euphoria.Engine.Entities;
using Euphoria.Engine.Entities.Components;
using Euphoria.Engine.InputSystem;
using Euphoria.Engine.Scenes;
using Euphoria.Physics;

namespace Tests.Engine.Components;

public class InteractComponent : Component
{
    private InputAction _interact;
    
    public Entity HeldEntity;

    public override void Initialize()
    {
        _interact = Input.GetActionSet("Main").GetAction("Interact");
    }

    public override void Update(float dt)
    {
        if (HeldEntity != null)
        {
            Rigidbody rb = HeldEntity.GetComponent<Rigidbody>();
            rb.ResetVelocity();
            rb.Teleport(Transform.Position + Transform.Forward * 3, Quaternion.Identity);

            if (_interact.IsPressed)
                HeldEntity = null;
            
            return;
        }
        
        if (PhysicsWorld.Raycast(Transform.Position, Transform.Forward, 100, out RayHit hit))
        {
            Entity hitEntity = hit.Entity();
            
            ref Transform transform = ref SceneManager.ActiveScene.GetEntity("HitCube").Transform;
            transform.Position = hit.Position;
            transform.Rotation = hitEntity.Transform.Rotation;

            if (hitEntity.TryGetComponent(out HighlightComponent highlight))
                highlight.Highlight = true;
            
            if (_interact.IsPressed && hitEntity.HasComponent<HighlightComponent>())
                HeldEntity = hitEntity;
        }
    }
}