namespace Euphoria.Engine.InputSystem.Bindings;

public interface IInputBinding<TValue> : IInputBindingBase
{
    public TValue Value { get; }
}