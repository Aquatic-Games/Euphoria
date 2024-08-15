namespace Euphoria.Engine.InputSystem.Bindings;

public interface IInputBindingBase
{
    public BindingType Type { get; }
    
    public bool IsDown { get; }
    
    public bool IsPressed { get; }
    
    public string AsConfigString();
}