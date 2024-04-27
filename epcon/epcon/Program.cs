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

string contentFile = args[0];

Console.WriteLine($"Processing file {contentFile}");
ContentFile file = JsonConvert.DeserializeObject<ContentFile>(File.ReadAllText(contentFile));
file.Items ??= [];

string contentFileLoc = Path.GetDirectoryName(contentFile);
string outDir = Path.Combine(contentFileLoc, file.OutDir);
Console.WriteLine($"Full output path: {outDir}");

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
List<IContentItemBase> items = new List<IContentItemBase>();

foreach (Dictionary<string, object> itemJson in file.Items)
{
    string name = (string) itemJson["Name"];
    Console.WriteLine($"Processing \"{name}\"");
    
    Type type = contentTypes[(string) itemJson["Type"]];
    IContentItemBase item = (IContentItemBase) Activator.CreateInstance(type);
    item.Name = name;
    
    foreach ((string key, object value) in itemJson)
    {
        if (key is "Type" or "Name")
            continue;

        PropertyInfo property = type.GetProperty(key);
        if (property == null)
            throw new Exception($"Could not find property with name \"{key}\".");

        if (property.PropertyType.IsEnum)
            property.SetValue(item, Enum.Parse(property.PropertyType, (string) value, true));
        else
            property.SetValue(item, value);
    }
    
    items.Add(item);
}

Console.WriteLine("Creating content info.");

ContentInfo info = new ContentInfo()
{
    OutputDirectory = outDir,
    Items = items.ToArray()
};

Console.WriteLine("Initializing builder.");

using Builder builder = new Builder(info);
Console.WriteLine("Building.");
builder.Build();