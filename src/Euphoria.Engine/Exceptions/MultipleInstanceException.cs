using System;

namespace Euphoria.Engine.Exceptions;

public class MultipleInstanceException : Exception
{
    public MultipleInstanceException(string iName) : base($"An instance of {iName} is already running.") { }
}