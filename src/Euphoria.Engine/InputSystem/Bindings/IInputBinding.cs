namespace Euphoria.Engine.InputSystem.Bindings;

public interface IInputBinding
{
    public bool IsDown { get; }
    
    public bool IsPressed { get; }
    
    public float Value { get; }

    public void Update();
}