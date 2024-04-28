using System.IO;
using grabs.Graphics;

namespace Euphoria.ContentBuilder.Items;

public struct DDSContent : IContentItem
{
    public string Name { get; set; }
    
    public string Path { get; set; }
    
    public Format Format { get; set; }
    
    public bool GenerateMips { get; set; }

    public DDSContent()
    {
        Format = Format.R8G8B8A8_UNorm;
        GenerateMips = true;
    }
    
    public ValidateResult Validate()
    {
        if (Path == null)
            return ValidateResult.Failure("Path is null.");
        
        if (!File.Exists(Path))
            return ValidateResult.Failure($"File \"{Path}\" could not be found. Does it exist?");
        
        return ValidateResult.Success;
    }

    public static string ItemType => "dds";

    public static string FileTypeBlob => "*.png | *.jpg | *.bmp | *.tga";
}