using System;

namespace Euphoria.Render;

public enum RenderType
{
    /// <summary>
    /// Only the texture (UI) renderer will be created. Use this option if you only intend to render UI.
    /// </summary>
    None,
    
    /// <summary>
    /// Only the texture (UI) and 2D renderer will be created. Use this option if you plan to make a 2D-only application.
    /// </summary>
    Only2D,
    
    /// <summary>
    /// Only the texture (UI) and 3D renderer will be created. Use this option if you plan to make a 3D-only application.
    /// </summary>
    Only3D,
    
    /// <summary>
    /// Create the texture (UI), 2D, and 3D renderer. Use this option if you plan to create an application using both 2D
    /// and 3D elements.
    /// </summary>
    Both
}