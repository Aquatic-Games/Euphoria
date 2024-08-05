using Euphoria.Parsers;

namespace Euphoria.Engine.Configs;

public interface ISerializableConfig<T>
{
    public void WriteIni(Ini ini);

    public static abstract bool TryFromIni(Ini ini, out T config);
}