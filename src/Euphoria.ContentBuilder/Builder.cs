using System;
using Euphoria.ContentBuilder.Items;

namespace Euphoria.ContentBuilder;

public class Builder
{
    public Builder(ContentInfo info)
    {
        foreach (IContentItem item in info.Items)
        {
            if (!item.Validate())
                throw new Exception("Failed to validate content item!");
        }
    }
}