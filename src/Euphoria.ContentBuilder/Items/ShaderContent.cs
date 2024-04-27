namespace Euphoria.ContentBuilder.Items;

public struct ShaderContent : IContentItem
{
    public static string ItemType => "shader";
    
    public string Name { get; set; }
    
    public string Path { get; set; }
    
    public string VEntry { get; set; }
    
    public string PEntry { get; set; }
    
    public bool Validate()
    {
        return Name != null;
    }
}