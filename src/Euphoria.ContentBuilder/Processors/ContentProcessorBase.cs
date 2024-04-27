using System;
using Euphoria.ContentBuilder.Items;

namespace Euphoria.ContentBuilder.Processors;

public abstract class ContentProcessorBase : IDisposable
{
    public abstract void Process(IContentItemBase item, string name, string outDir);

    public virtual void Dispose() { }
}