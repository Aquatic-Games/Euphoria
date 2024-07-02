using System;

namespace Euphoria.Render;

[Flags]
public enum UpdateFlags
{
    None,
    
    /// <summary>
    /// Marks that the renderable can be updated.
    /// </summary>
    Updatable = 1 << 0,
    
    /// <summary>
    /// Do not recreate buffers if update is larger than the current buffers.
    /// </summary>
    NoRecreateBuffers = 1 << 1,
    
    /// <summary>
    /// If the buffers are recreated on update, their size will be doubled, or resized to fit the data, whichever is
    /// larger. If <b>not</b> set, the buffers will only be sized to exactly fit the new data.
    /// </summary>
    ExpandBuffers = 1 << 2,
    
    /// <summary>
    /// Marks that the renderable will be updated often.
    /// </summary>
    Dynamic = Updatable | ExpandBuffers
}