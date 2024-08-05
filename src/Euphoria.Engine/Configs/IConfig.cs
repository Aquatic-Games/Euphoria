namespace Euphoria.Engine.Configs;

public interface IConfig<T> : ISerializableConfig<T>
{
    public void Save(string path);
    
    public static abstract T CurrentConfig { get; set; }

    public static abstract bool TryLoadFromFile(string path, out T config);
}