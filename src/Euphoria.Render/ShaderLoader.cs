using System;
using grabs.Graphics;

namespace Euphoria.Render;

public class ShaderLoader
{
    private readonly Func<string, ShaderStage, byte[]> LoadSpirvShaderFunc;
    
    public ShaderLoader(Func<string, ShaderStage, byte[]> loadSpirvShaderFunc)
    {
        LoadSpirvShaderFunc = loadSpirvShaderFunc;
    }

    public byte[] LoadSpirvShader(string shaderName, ShaderStage stage)
        => LoadSpirvShaderFunc(shaderName, stage);
}