using Euphoria.ContentBuilder.Items;

namespace Euphoria.ContentBuilder.Processors;

public abstract class ContentProcessor<TItem> : ContentProcessorBase where TItem : IContentItem
{
    public abstract void Process(TItem item, string name, string outDir);

    public override void Process(IContentItem item, string name, string outDir)
        => Process((TItem) item, name, outDir);
}