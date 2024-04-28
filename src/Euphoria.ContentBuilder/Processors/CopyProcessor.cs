using System;
using System.IO;
using Euphoria.ContentBuilder.Items;

namespace Euphoria.ContentBuilder.Processors;

public class CopyProcessor : ContentProcessor<CopyContent>
{
    public override void Process(CopyContent item, string name, string outDir)
    {
        string dest = Path.Combine(outDir, name + Path.GetExtension(item.Path));
        
        Console.WriteLine($"Copying file {item.Path} to {dest}");
        File.Copy(item.Path, dest);
    }
}