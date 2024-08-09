namespace Euphoria.Engine.InputSystem.Bindings;

public interface IInputBinding<TValue>
{
    public bool IsDown { get; }
    
    public bool IsPressed { get; }
    
    public TValue Value { get; }
}