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
    
    public bool Validate()
    {
        return Name != null && Path != null;
    }

    public static string ItemType => "dds";
}