using System;
using System.Collections.Generic;
using System.Text;

namespace Euphoria.Parsers;

public sealed class Ini
{
    public Dictionary<string, Item> Items;
    
    public Dictionary<string, Group> Groups;

    public Ini()
    {
        Items = new Dictionary<string, Item>();
        Groups = new Dictionary<string, Group>();
    }
    
    public Ini(Dictionary<string, Item> items, Dictionary<string, Group> groups)
    {
        Items = items;
        Groups = groups;
    }

    public Ini(string ini, LoadFlags flags = LoadFlags.None)
    {
        string[] splitIni = ini.Trim().Split("\n");

        int lineNum = 0;
        Items = new Dictionary<string, Item>();
        Groups = new Dictionary<string, Group>();
        Group currentGroup = new Group();

        bool stringOnly = (flags & LoadFlags.StringOnly) == LoadFlags.StringOnly;
        
        foreach (string l in splitIni)
        {
            lineNum++;
            string line = l.Trim();
            
            // Ignore blank lines and comments
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith('#'))
                continue;

            // Group
            if (line.StartsWith('['))
            {
                if (!line.EndsWith(']'))
                    throw new Exception($"Line {lineNum}: Expected ']'.");

                if (currentGroup.Name == "" && currentGroup.Items.Count > 0)
                    Items = currentGroup.Items;
                else if (currentGroup.Name != "")
                    Groups.TryAdd(currentGroup.Name, currentGroup);

                // Append to group if it exists, instead of overwriting.
                string groupName = line.TrimStart('[').TrimEnd(']').Trim();
                if (!Groups.TryGetValue(groupName, out currentGroup))
                {
                    if (string.IsNullOrWhiteSpace(groupName))
                        throw new Exception($"Line {lineNum}: Group name cannot be empty.");

                    currentGroup = new Group();
                    currentGroup.Name = groupName;
                }
                
                continue;
            }

            int equalsIndex = line.IndexOf('=');
            if (equalsIndex == -1)
                throw new Exception($"Line {lineNum}: Expected '='.");

            string key = line.Substring(0, equalsIndex).Trim();
            if (key.Length == 0)
                throw new Exception($"Line {lineNum}: Expected key.");
            if (currentGroup.Items.ContainsKey(key))
                throw new Exception($"Line {lineNum}: Item with key '{key}' already exists.");
            
            string value = line.Substring(equalsIndex + 1).Trim();

            double dValue;
            bool bValue;
            
            if (stringOnly)
                currentGroup.Items.Add(key, new Item(ItemType.String, value.Trim('\'', '"')));
            else
            {
                if (value.Length == 0 || value == "null" && !stringOnly)
                    currentGroup.Items.Add(key, new Item(ItemType.Null, null));
                else if (!stringOnly && double.TryParse(value, out dValue))
                    currentGroup.Items.Add(key, new Item(ItemType.Number, dValue));
                else if (!stringOnly && bool.TryParse(value, out bValue))
                    currentGroup.Items.Add(key, new Item(ItemType.Boolean, bValue));
                else
                    currentGroup.Items.Add(key, new Item(ItemType.String, value.Trim('\'', '"')));
            }
        }
        
        if (currentGroup.Name == "" && currentGroup.Items.Count > 0)
            Items = currentGroup.Items;
        else if (currentGroup.Name != "")
            Groups.TryAdd(currentGroup.Name, currentGroup);
    }

    public string Serialize(SerializeFlags flags = SerializeFlags.SpacesBetweenGroups)
    {
        StringBuilder builder = new StringBuilder();
        
        if (Items != null)
            AddItems(ref builder, Items, flags);

        if (Groups != null)
        {
            bool spacesBetweenGroups = 
                (flags & SerializeFlags.SpacesBetweenGroups) == SerializeFlags.SpacesBetweenGroups;

            foreach (KeyValuePair<string, Group> group in Groups)
            {
                if (spacesBetweenGroups)
                    builder.AppendLine();

                builder.AppendLine($"[{group.Key}]");
                AddItems(ref builder, group.Value.Items, flags);
            }
        }
        

        return builder.ToString();
    }

    private static void AddItems(ref StringBuilder builder, Dictionary<string, Item> items, SerializeFlags flags)
    {
        bool spacesBetweenEquals =
            (flags & SerializeFlags.SpacesBetweenEquals) == SerializeFlags.SpacesBetweenEquals;
        
        foreach (KeyValuePair<string, Item> item in items)
        {
            builder.Append(item.Key);

            if (spacesBetweenEquals)
                builder.Append(" = ");
            else
                builder.Append('=');

            switch (item.Value.Type)
            {
                case ItemType.Null:
                    builder.AppendLine("null");
                    break;
                case ItemType.Boolean:
                    builder.AppendLine(item.Value.ToString().ToLower());
                    break;
                default:
                    builder.AppendLine(item.Value.ToString());
                    break;
            }
        }
    }
    
    [Flags]
    public enum LoadFlags
    {
        None,
    
        StringOnly = 1 << 0
    }

    [Flags]
    public enum SerializeFlags
    {
        None = 0,
    
        SpacesBetweenEquals = 1 << 0,
        SpacesBetweenGroups = 1 << 1
    }

    public class Group
    {
        public string Name;
    
        public Dictionary<string, Item> Items;

        public Group()
        {
            Name = "";
            Items = new Dictionary<string, Item>();
        }

        public Group(string name, Dictionary<string, Item> items)
        {
            Name = name;
            Items = items;
        }
    }

    public class Item
    {
        public ItemType Type;
        public object Value;

        public Item(ItemType type, object value)
        {
            Type = type;
            Value = value;
        }

        public override string ToString()
        {
            return Value?.ToString() ?? "Null";
        }
    }

    public enum ItemType
    {
        Null,
        String,
        Number,
        Boolean,
    }
}