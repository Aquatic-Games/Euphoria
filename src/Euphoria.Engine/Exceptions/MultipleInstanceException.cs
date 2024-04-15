using System;

namespace u4.Engine.Exceptions;

public class MultipleInstanceException : Exception
{
    public MultipleInstanceException(string iName) : base($"An instance of {iName} is already running.") { }
}