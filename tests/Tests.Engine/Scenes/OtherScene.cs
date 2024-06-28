using Euphoria.Engine;
using Euphoria.Engine.Scenes;

namespace Tests.Engine.Scenes;

public class OtherScene : Scene
{
    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.IsKeyPressed(Key.O))
            SceneManager.LoadAndSwitchScene(new TestScene());
    }
}