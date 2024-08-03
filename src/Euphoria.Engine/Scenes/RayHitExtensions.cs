using Euphoria.Engine.Entities;
using Euphoria.Physics;

namespace Euphoria.Engine.Scenes;

public static class RayHitExtensions
{
    public static Entity Entity(this RayHit hit)
    {
        if (SceneManager.ActiveScene.BodyIdToEntity.TryGetValue(hit.Body.Id, out Entity entity))
            return entity;

        return null;
    }
}