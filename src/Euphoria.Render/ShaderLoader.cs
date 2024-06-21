using System;
using System.IO;
using System.Reflection;
using grabs.Graphics;

namespace Euphoria.Render;

public static class ShaderLoader
{
    public static byte[] LoadSpirvShader(string shaderName, ShaderStage stage)
    {
        string resourceName = ShaderLocationBase + '.' + shaderName.Replace('/', '.');
        resourceName += stage switch
        {
            ShaderStage.Vertex => "_v.spv",
            ShaderStage.Pixel => "_p.spv",
            ShaderStage.Compute => "_c.spv",
            ShaderStage.All => throw new NotSupportedException(),
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        Assembly assembly = Assembly.GetCallingAssembly();
        using Stream stream = assembly.GetManifestResourceStream(resourceName);
        if (stream == null)
            throw new Exception($"Could not find a shader with name {shaderName}. (Resource name: {resourceName})");
        
        using MemoryStream resource = new MemoryStream();
        stream.CopyTo(resource);

        byte[] result = resource.ToArray();
        return result;
    }

    public const string ShaderLocationBase = "Euphoria.Render.Shaders";
}