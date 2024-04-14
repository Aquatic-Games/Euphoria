using System;

namespace Euphoria.Render.GL;

public class GLGraphics : Graphics
{
    private Action<int> _presentFunc;
    
    public Silk.NET.OpenGL.GL GL;
    
    public override RenderAPI RenderAPI => RenderAPI.OpenGL;
    
    public GLGraphics(Func<string, nint> getProcAddressFunc, Action<int> presentFunc)
    {
        _presentFunc = presentFunc;
        
        GL = Silk.NET.OpenGL.GL.GetApi(getProcAddressFunc);
    }

    public override void Present()
    {
        _presentFunc(1);
    }

    public override void Dispose()
    {
        GL.Dispose();
    }
}