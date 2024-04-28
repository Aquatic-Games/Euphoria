using System.IO;

namespace Euphoria.ContentBuilder.Items;

public struct ShaderContent : IContentItem
{
    public static string ItemType => "shader";
    
    public string Name { get; set; }
    
    public string Path { get; set; }
    
    public string VEntry { get; set; }
    
    public string PEntry { get; set; }
    
    public ValidateResult Validate()
    {
        if (Path == null)
            return ValidateResult.Failure("Path is null.");
        
        if (!File.Exists(Path))
            return ValidateResult.Failure($"File \"{Path}\" could not be found. Does it exist?");

        if (System.IO.Path.GetExtension(Path).ToLower() != "hlsl")
        {
            return ValidateResult.Failure(
                $"File \"{Path}\" is not an HLSL file. Euphoria can only compile HLSL shaders. If this *is* an HLSL file, please use the \".hlsl\" file extension. NOTE: Euphoria cannot compile .fx files, and .hlsli files should not be added to the content processor.");
        }
        
        if (string.IsNullOrWhiteSpace(VEntry))
            return ValidateResult.Failure("Shader file MUST contain a valid Vertex shader entry point.");
        
        if (string.IsNullOrWhiteSpace(PEntry))
            return ValidateResult.Failure("Shader file MUST contain a valid Pixel shader entry point.");

        return ValidateResult.Success;
    }
}