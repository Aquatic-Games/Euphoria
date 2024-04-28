using System.IO;

namespace Euphoria.ContentBuilder.Items;

public struct CopyContent : IContentItem
{
    public string Name { get; set; }
    
    public string Path { get; set; }
    
    public ValidateResult Validate()
    {
        if (Path == null)
            return ValidateResult.Failure("Path is null.");
        
        if (!File.Exists(Path))
            return ValidateResult.Failure($"File \"{Path}\" could not be found. Does it exist?");
        
        return ValidateResult.Success;
    }

    public static string ItemType => "copy";
}