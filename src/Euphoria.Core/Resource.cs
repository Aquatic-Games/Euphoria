using System;
using System.IO;
using System.Reflection;

namespace Euphoria.Core;

public static class Resource
{
    public static byte[] LoadEmbedded(string name, Assembly assembly)
    {
        Stream stream = assembly.GetManifestResourceStream(name);

        if (stream == null)
            throw new Exception($"Could not load resource \"{name}\" in assembly {assembly}.");

        MemoryStream memStream = new MemoryStream();
        stream.CopyTo(memStream);

        return memStream.ToArray();
    }
}