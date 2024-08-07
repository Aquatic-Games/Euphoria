namespace Euphoria.Engine.InputSystem.Bindings;

public struct DualKeyBinding : IInputBinding
{
    public Key Positive;

    public Key Negative;
    
    public bool IsDown { get; private set; }
    
    public bool IsPressed { get; private set; }
    
    public float Value { get; private set; }

    public DualKeyBinding(Key positive, Key negative)
    {
        Positive = positive;
        Negative = negative;
    }

    public void Update()
    {
        Value = 0;
        
        if (Input.IsKeyDown(Positive))
            Value += 1;

        if (Input.IsKeyDown(Negative))
            Value -= 1;
    }
}