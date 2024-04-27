using Euphoria.ContentBuilder.Items;

namespace Euphoria.ContentBuilder.Processors;

public abstract class ContentProcessorBase
{
    public abstract void Process(IContentItemBase item, string name, string outDir);
}