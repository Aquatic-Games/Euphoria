using Euphoria.Parsers;

namespace Euphoria.Engine.Configs;

public interface ISerializableConfig<out T>
{
    public void WriteIni(Ini ini);

    public static abstract T FromIni(Ini ini);
}