using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Euphoria.ContentBuilder.Items;
using Euphoria.ContentBuilder.Processors;

namespace Euphoria.ContentBuilder;

public class Builder
{
    private ContentInfo _info;
    private Dictionary<Type, ContentProcessorBase> _processors;
    
    public Builder(ContentInfo info)
    {
        _info = info;
        
        Console.WriteLine("Validating items.");
        foreach (IContentItem item in _info.Items)
        {
            if (!item.Validate())
                throw new Exception($"Failed to validate content item \"{item.Name}\"");
        }
        
        Console.WriteLine("Detecting content processors.");
        _processors = new Dictionary<Type, ContentProcessorBase>();
        
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()
                     .Where(type => type.IsAssignableTo(typeof(ContentProcessorBase)) && type.BaseType.IsGenericType))
        {
            Console.WriteLine($"Found {type}");
            
            Type processorContentType = type.BaseType.GenericTypeArguments[0];
            ContentProcessorBase processor = (ContentProcessorBase) Activator.CreateInstance(type);
            
            _processors.Add(processorContentType, processor);
        }
    }

    public void Build()
    {
        Console.WriteLine("Beginning build.");
        
        foreach (IContentItem item in _info.Items)
        {
            Console.WriteLine($"Building item \"{item.Name}\"");

            string[] splitItemName = item.Name.Split('/');
            string itemName = splitItemName[^1];
            string outDir = Path.Combine(_info.OutputDirectory, Path.Combine(splitItemName[..^1]));

            Directory.CreateDirectory(outDir);
            
            _processors[item.GetType()].Process(item, itemName, outDir);
        }
        
        Console.WriteLine("Build complete.");
    }
}