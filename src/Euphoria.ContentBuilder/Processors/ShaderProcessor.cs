using System;
using System.IO;
using Euphoria.ContentBuilder.Items;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;
using u4.Core;

namespace Euphoria.ContentBuilder.Processors;

public class ShaderProcessor : ContentProcessor<ShaderContent>
{
    public override void Process(ShaderContent item, string name, string outDir)
    {
        string hlsl = File.ReadAllText(item.Path);

        Logger.Trace("Compiling vertex shader.");
        byte[] vertSpv = Compiler.CompileToSpirV(hlsl, item.VEntry, ShaderStage.Vertex, true);
        
        Logger.Trace("Compiling pixel shader.");
        byte[] pixlSpv = Compiler.CompileToSpirV(hlsl, item.PEntry, ShaderStage.Pixel, true);
        
        Logger.Trace("Outputting to directory.");
        File.WriteAllBytes(Path.Combine(outDir, $"{name}_v.spv"), vertSpv);
        File.WriteAllBytes(Path.Combine(outDir, $"{name}_p.spv"), pixlSpv);
    }
}