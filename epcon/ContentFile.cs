namespace epcon;

public class ContentFile
{
    public string OutDirName;

    public string OutDirPath;

    public static ContentFile Default => new ContentFile()
    {
        OutDirName = "Content",
        OutDirPath = null
    };
}