using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using Euphoria.ContentBuilder.Items;
using Euphoria.ContentBuilder.Processors;
using u4.Core;

namespace Euphoria.ContentBuilder;

public class Builder : IDisposable
{
    private ContentInfo _info;
    private Dictionary<Type, ContentProcessorBase> _processors;
    
    public Builder(ContentInfo info)
    {
        _info = info;
        
        Logger.Info("Validating items.");
        foreach (IContentItemBase item in _info.Items)
        {
            ValidateResult result = item.Validate();
            if (!result.Succeeded)
                throw new Exception($"Failed to validate content item \"{item.Name}\": {result.FailureReason}");
        }
        
        Logger.Info("Detecting content processors.");
        _processors = new Dictionary<Type, ContentProcessorBase>();
        
        foreach (Type type in Assembly.GetExecutingAssembly().GetTypes()
                     .Where(type => type.IsAssignableTo(typeof(ContentProcessorBase)) && type.BaseType.IsGenericType))
        {
            Logger.Debug($"Found {type}");
            
            Type processorContentType = type.BaseType.GenericTypeArguments[0];
            ContentProcessorBase processor = (ContentProcessorBase) Activator.CreateInstance(type);
            
            _processors.Add(processorContentType, processor);
        }
    }

    public void Build()
    {
        Logger.Info("Beginning build.");

        List<IContentItemBase> contentItems = new List<IContentItemBase>(_info.Items);
        
        foreach (IContentItemBase item in contentItems)
        {
            Logger.Info($"Building item \"{item.Name}\"");

            string[] splitItemName = item.Name.Split('/');
            string itemName = splitItemName[^1];
            string outDir = Path.Combine(_info.OutputDirectory, Path.Combine(splitItemName[..^1]));

            Directory.CreateDirectory(outDir);
            
            _processors[item.GetType()].Process(item, itemName, outDir);
        }
        
        Logger.Info("Build complete.");
    }

    public void Dispose()
    {
        foreach ((_, ContentProcessorBase processor) in _processors)
            processor.Dispose();
    }
}