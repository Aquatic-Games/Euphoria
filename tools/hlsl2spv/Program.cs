using System;
using System.IO;
using grabs.Graphics;
using grabs.ShaderCompiler.DXC;

bool deleteFiles = true;

Console.WriteLine("hlsl2spv");
Console.WriteLine("Aquatic Games 2024");

Console.WriteLine($"Command line arguments: [{string.Join(", ", args)}]");

string inDirectory = Environment.CurrentDirectory;

for (int i = 0; i < args.Length; i++)
{
    string arg = args[i];

    if (arg.StartsWith('-'))
    {
        switch (arg)
        {
            case "--no-delete":
                deleteFiles = false;
                break;
            
            default:
                throw new Exception($"Unrecognized argument \"{arg}\"");
        }
    }
    else if (Directory.Exists(arg))
        inDirectory = arg;
    else
        throw new Exception($"Unrecognized argument \"{arg}\"");
}

Console.WriteLine($"Delete files: {(deleteFiles ? "ON" : "OFF")}");
Console.WriteLine($"Directory: {inDirectory}");

if (deleteFiles)
{
    Console.WriteLine("Deleting files.");

    foreach (string file in Directory.GetFiles(inDirectory, "*.spv", SearchOption.AllDirectories))
    {
        Console.WriteLine($"Deleting {file}");
        File.Delete(file);
    }
}

Console.WriteLine("Compiling shaders.");

foreach (string file in Directory.GetFiles(inDirectory, "*.hlsl", SearchOption.AllDirectories))
{
    Console.WriteLine($"Compiling {file}: ");
    
    string shader = File.ReadAllText(file);

    string vertexEntryPoint = null;
    string pixelEntryPoint = null;
    bool debug = false;
    
    // Preprocess shader, get the entry points of various shader types. Can be expanded later.
    foreach (string line in shader.Split('\n'))
    {
        if (!line.StartsWith("#pragma"))
            continue;
        
        string[] splitLine = line.Trim().Split(' ');

        switch (splitLine[1])
        {
            case "vertex":
            {
                vertexEntryPoint = splitLine[2];
                break;
            }
            
            case "pixel":
            {
                pixelEntryPoint = splitLine[2];
                break;
            }
            
            case "debug":
                debug = true;
                break;
        }
    }
    
    Console.WriteLine(debug);

    if (vertexEntryPoint == null && pixelEntryPoint == null)
        throw new Exception($"{file}: Shader must include AT LEAST one entry point, or must be .hlsli file.");
    
    string fileDir = Path.GetDirectoryName(file);
    string fileName = Path.GetFileNameWithoutExtension(file);
    string outFileName = Path.Combine(fileDir, fileName);

    if (vertexEntryPoint != null)
    {
        Console.Write("    Vertex... ");
        byte[] result = Compiler.CompileToSpirV(shader, vertexEntryPoint, ShaderStage.Vertex, debug, [fileDir]);
        File.WriteAllBytes(outFileName + "_v.spv", result);
        Console.WriteLine("Done.");
    }

    if (pixelEntryPoint != null)
    {
        Console.Write("    Pixel... ");
        byte[] result = Compiler.CompileToSpirV(shader, pixelEntryPoint, ShaderStage.Pixel, debug, [fileDir]);
        File.WriteAllBytes(outFileName + "_p.spv", result);
        Console.WriteLine("Done.");
    }
}

Console.WriteLine("Compilation successful.");