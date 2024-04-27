namespace Euphoria.ContentBuilder.Items;

public interface IContentItemBase
{
    public string Name { get; set; }
    
    public bool Validate();
}