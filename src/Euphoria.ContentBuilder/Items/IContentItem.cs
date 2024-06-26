﻿namespace Euphoria.ContentBuilder.Items;

public interface IContentItem : IContentItemBase
{
    /// <summary>
    /// This is a unique <b>item type</b> string, used in the <i>epcon</i> manager. 
    /// </summary>
    public static abstract string ItemType { get; }
    
    /// <summary>
    /// All file types this content item type processes.
    /// </summary>
    public static abstract string FileTypeBlob { get; }
}