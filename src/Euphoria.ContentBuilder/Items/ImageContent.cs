namespace Euphoria.ContentBuilder.Items;

public struct ImageContent : IContentItem
{
    public string Name { get; set; }
    
    public string Path { get; set; }
    
    public bool Validate()
    {
        return Name != null;
    }

    public static string ItemType => "image";
}