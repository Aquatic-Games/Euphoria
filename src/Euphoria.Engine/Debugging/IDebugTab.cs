namespace Euphoria.Engine.Debugging;

public interface IDebugTab
{
    public string TabName { get; }

    internal void Update();

    internal void Draw() { }
}