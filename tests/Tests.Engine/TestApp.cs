using Euphoria.Engine;

namespace Tests.Engine;

public class TestApp : Application
{
    public override void Update(float dt)
    {
        base.Update(dt);
        
        if (Input.IsKeyPressed(Key.Escape))
            App.Close();
    }
}