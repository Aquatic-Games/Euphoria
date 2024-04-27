using Euphoria.ContentBuilder.Items;

namespace Euphoria.ContentBuilder.Processors;

public abstract class ContentProcessorBase
{
    public abstract void Process(IContentItem item, string name, string outDir);
}