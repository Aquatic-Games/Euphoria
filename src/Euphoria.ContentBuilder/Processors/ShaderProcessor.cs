using System;
using System.IO;
using Euphoria.ContentBuilder.Items;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;

namespace Euphoria.ContentBuilder.Processors;

public class ShaderProcessor : ContentProcessor<ShaderContent>
{
    public override void Process(ShaderContent item, string name, string outDir)
    {
        string hlsl = File.ReadAllText(item.Path);

        Console.WriteLine("Compiling vertex shader.");
        byte[] vertSpv = Compiler.CompileToSpirV(hlsl, item.VEntry, ShaderStage.Vertex, true);
        
        Console.WriteLine("Compiling pixel shader.");
        byte[] pixlSpv = Compiler.CompileToSpirV(hlsl, item.PEntry, ShaderStage.Pixel, true);
        
        Console.WriteLine("Outputting to directory.");
        File.WriteAllBytes(Path.Combine(outDir, $"{name}_v.spv"), vertSpv);
        File.WriteAllBytes(Path.Combine(outDir, $"{name}_p.spv"), pixlSpv);
    }
}