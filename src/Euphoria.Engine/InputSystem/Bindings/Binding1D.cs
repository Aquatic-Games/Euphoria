﻿using System;
using System.Numerics;

namespace Euphoria.Engine.InputSystem.Bindings;

public struct Binding1D : IInputBinding
{
    public IInputBinding Positive;

    public IInputBinding Negative;

    public BindingType Type => Positive switch
    {
        KeyBinding k => BindingType.Key1D,
        _ => throw new ArgumentOutOfRangeException()
    };
    
    public bool IsDown => Positive.IsDown || Negative.IsDown;

    public bool IsPressed => Positive.IsPressed || Negative.IsPressed;
    
    public Vector3 Value => new(Positive.Value.X - Negative.Value.X);

    public Binding1D(IInputBinding positive, IInputBinding negative)
    {
        Positive = positive;
        Negative = negative;
    }

    public string AsString()
        => $"Positive:{Positive.AsString()};Negative:{Negative.AsString()}";
}