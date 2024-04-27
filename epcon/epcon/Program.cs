using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using epcon;
using Euphoria.ContentBuilder;
using Euphoria.ContentBuilder.Items;
using Newtonsoft.Json;

Console.WriteLine("EPCON content manager.");

string contentFile = File.ReadAllText(args[0]);
Console.WriteLine($"Processing file {contentFile}");

ContentFile file = JsonConvert.DeserializeObject<ContentFile>(contentFile);

Assembly contentBuilderAssembly = Assembly.GetAssembly(typeof(Builder));

Console.WriteLine("Detecting available content types...");
Dictionary<string, Type> contentTypes = new Dictionary<string, Type>();

Type contentItemType = typeof(IContentItem);
foreach (Type type in contentBuilderAssembly.GetTypes().Where(type => type.IsAssignableTo(contentItemType) && type != contentItemType))
{
    string jsonTypeName = (string) type.GetProperty("ItemType", BindingFlags.Public | BindingFlags.Static).GetValue(null);
    Console.WriteLine($"Found \"{jsonTypeName}\" ({type})");
    contentTypes.Add(jsonTypeName, type);
}

Console.WriteLine("Creating content items from JSON.");
List<object> items = new List<object>();

foreach (Dictionary<string, object> itemJson in file.Items)
{
    string name = (string) itemJson["Name"];
    Console.WriteLine($"Processing \"{name}\"");
    
    Type type = contentTypes[(string) itemJson["Type"]];
    IContentItem item = (IContentItem) Activator.CreateInstance(type);
    item.Name = name;
    
    foreach ((string key, object value) in itemJson)
    {
        if (key is "Type" or "Name")
            continue;

        PropertyInfo property = type.GetProperty(key);
        if (property == null)
            throw new Exception($"Could not find property with name \"{key}\".");

        property.SetValue(item, value);
    }
    
    items.Add(item);
}

Console.WriteLine("Creating content info.");

// There's probably better ways to do this. But it works.
IContentItem[] itemArray = new IContentItem[items.Count];
for (int i = 0; i < itemArray.Length; i++)
    itemArray[i] = (IContentItem) items[i];

ContentInfo info = new ContentInfo()
{
    OutputDirectory = file.OutputDir,
    Items = itemArray
};

Console.WriteLine("Initializing builder.");

Builder builder = new Builder(info);