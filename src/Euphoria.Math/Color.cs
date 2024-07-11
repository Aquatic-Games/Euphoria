using System.Numerics;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Euphoria.Math;

[StructLayout(LayoutKind.Sequential)]
public struct Color
{
    public float R;
    public float G;
    public float B;
    public float A;

    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }

    public Color(byte r, byte g, byte b, byte a = byte.MaxValue)
    {
        R = r / (float) byte.MaxValue;
        G = g / (float) byte.MaxValue;
        B = b / (float) byte.MaxValue;
        A = a / (float) byte.MaxValue;
    }

    public Color(uint packedRgba)
    {
        R = (packedRgba >> 24) / (float) byte.MaxValue;
        G = ((packedRgba >> 16) & 0xFF) / (float) byte.MaxValue;
        B = ((packedRgba >> 8) & 0xFF) / (float) byte.MaxValue;
        A = (packedRgba & 0xFF) / (float) byte.MaxValue;
    }

    public Color(Color color, float alpha)
    {
        R = color.R;
        G = color.G;
        B = color.B;
        A = alpha;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static explicit operator Vector4(Color color)
        => new Vector4(color.R, color.G, color.B, color.A);
    
    // TODO: HSV
    
    /// <summary>
    /// AliceBlue has an RGBA value of #F0F8FFFF (240, 248, 255, 255)
    /// </summary>
    public static Color AliceBlue => new Color(0.9411764705882353f, 0.9725490196078431f, 1.0f);

    /// <summary>
    /// AntiqueWhite has an RGBA value of #FAEBD7FF (250, 235, 215, 255)
    /// </summary>
    public static Color AntiqueWhite => new Color(0.9803921568627451f, 0.9215686274509803f, 0.8431372549019608f);

    /// <summary>
    /// Aqua has an RGBA value of #00FFFFFF (0, 255, 255, 255)
    /// </summary>
    public static Color Aqua => new Color(0.0f, 1.0f, 1.0f);

    /// <summary>
    /// Aquamarine has an RGBA value of #7FFFD4FF (127, 255, 212, 255)
    /// </summary>
    public static Color Aquamarine => new Color(0.4980392156862745f, 1.0f, 0.8313725490196079f);

    /// <summary>
    /// Azure has an RGBA value of #F0FFFFFF (240, 255, 255, 255)
    /// </summary>
    public static Color Azure => new Color(0.9411764705882353f, 1.0f, 1.0f);

    /// <summary>
    /// Beige has an RGBA value of #F5F5DCFF (245, 245, 220, 255)
    /// </summary>
    public static Color Beige => new Color(0.9607843137254902f, 0.9607843137254902f, 0.8627450980392157f);

    /// <summary>
    /// Bisque has an RGBA value of #FFE4C4FF (255, 228, 196, 255)
    /// </summary>
    public static Color Bisque => new Color(1.0f, 0.8941176470588236f, 0.7686274509803922f);

    /// <summary>
    /// Black has an RGBA value of #000000FF (0, 0, 0, 255)
    /// </summary>
    public static Color Black => new Color(0.0f, 0.0f, 0.0f);

    /// <summary>
    /// BlanchedAlmond has an RGBA value of #FFEBCDFF (255, 235, 205, 255)
    /// </summary>
    public static Color BlanchedAlmond => new Color(1.0f, 0.9215686274509803f, 0.803921568627451f);

    /// <summary>
    /// Blue has an RGBA value of #0000FFFF (0, 0, 255, 255)
    /// </summary>
    public static Color Blue => new Color(0.0f, 0.0f, 1.0f);

    /// <summary>
    /// BlueViolet has an RGBA value of #8A2BE2FF (138, 43, 226, 255)
    /// </summary>
    public static Color BlueViolet => new Color(0.5411764705882353f, 0.16862745098039217f, 0.8862745098039215f);

    /// <summary>
    /// Brown has an RGBA value of #A52A2AFF (165, 42, 42, 255)
    /// </summary>
    public static Color Brown => new Color(0.6470588235294118f, 0.16470588235294117f, 0.16470588235294117f);

    /// <summary>
    /// BurlyWood has an RGBA value of #DEB887FF (222, 184, 135, 255)
    /// </summary>
    public static Color BurlyWood => new Color(0.8705882352941177f, 0.7215686274509804f, 0.5294117647058824f);

    /// <summary>
    /// CadetBlue has an RGBA value of #5F9EA0FF (95, 158, 160, 255)
    /// </summary>
    public static Color CadetBlue => new Color(0.37254901960784315f, 0.6196078431372549f, 0.6274509803921569f);

    /// <summary>
    /// Chartreuse has an RGBA value of #7FFF00FF (127, 255, 0, 255)
    /// </summary>
    public static Color Chartreuse => new Color(0.4980392156862745f, 1.0f, 0.0f);

    /// <summary>
    /// Chocolate has an RGBA value of #D2691EFF (210, 105, 30, 255)
    /// </summary>
    public static Color Chocolate => new Color(0.8235294117647058f, 0.4117647058823529f, 0.11764705882352941f);

    /// <summary>
    /// Coral has an RGBA value of #FF7F50FF (255, 127, 80, 255)
    /// </summary>
    public static Color Coral => new Color(1.0f, 0.4980392156862745f, 0.3137254901960784f);

    /// <summary>
    /// CornflowerBlue has an RGBA value of #6495EDFF (100, 149, 237, 255)
    /// </summary>
    public static Color CornflowerBlue => new Color(0.39215686274509803f, 0.5843137254901961f, 0.9294117647058824f);

    /// <summary>
    /// Cornsilk has an RGBA value of #FFF8DCFF (255, 248, 220, 255)
    /// </summary>
    public static Color Cornsilk => new Color(1.0f, 0.9725490196078431f, 0.8627450980392157f);

    /// <summary>
    /// Crimson has an RGBA value of #DC143CFF (220, 20, 60, 255)
    /// </summary>
    public static Color Crimson => new Color(0.8627450980392157f, 0.0784313725490196f, 0.23529411764705882f);

    /// <summary>
    /// Cyan has an RGBA value of #00FFFFFF (0, 255, 255, 255)
    /// </summary>
    public static Color Cyan => new Color(0.0f, 1.0f, 1.0f);

    /// <summary>
    /// DarkBlue has an RGBA value of #00008BFF (0, 0, 139, 255)
    /// </summary>
    public static Color DarkBlue => new Color(0.0f, 0.0f, 0.5450980392156862f);

    /// <summary>
    /// DarkCyan has an RGBA value of #008B8BFF (0, 139, 139, 255)
    /// </summary>
    public static Color DarkCyan => new Color(0.0f, 0.5450980392156862f, 0.5450980392156862f);

    /// <summary>
    /// DarkGoldenRod has an RGBA value of #B8860BFF (184, 134, 11, 255)
    /// </summary>
    public static Color DarkGoldenRod => new Color(0.7215686274509804f, 0.5254901960784314f, 0.043137254901960784f);

    /// <summary>
    /// DarkGray has an RGBA value of #A9A9A9FF (169, 169, 169, 255)
    /// </summary>
    public static Color DarkGray => new Color(0.6627450980392157f, 0.6627450980392157f, 0.6627450980392157f);

    /// <summary>
    /// DarkGrey has an RGBA value of #A9A9A9FF (169, 169, 169, 255)
    /// </summary>
    public static Color DarkGrey => new Color(0.6627450980392157f, 0.6627450980392157f, 0.6627450980392157f);

    /// <summary>
    /// DarkGreen has an RGBA value of #006400FF (0, 100, 0, 255)
    /// </summary>
    public static Color DarkGreen => new Color(0.0f, 0.39215686274509803f, 0.0f);

    /// <summary>
    /// DarkKhaki has an RGBA value of #BDB76BFF (189, 183, 107, 255)
    /// </summary>
    public static Color DarkKhaki => new Color(0.7411764705882353f, 0.7176470588235294f, 0.4196078431372549f);

    /// <summary>
    /// DarkMagenta has an RGBA value of #8B008BFF (139, 0, 139, 255)
    /// </summary>
    public static Color DarkMagenta => new Color(0.5450980392156862f, 0.0f, 0.5450980392156862f);

    /// <summary>
    /// DarkOliveGreen has an RGBA value of #556B2FFF (85, 107, 47, 255)
    /// </summary>
    public static Color DarkOliveGreen => new Color(0.3333333333333333f, 0.4196078431372549f, 0.1843137254901961f);

    /// <summary>
    /// DarkOrange has an RGBA value of #FF8C00FF (255, 140, 0, 255)
    /// </summary>
    public static Color DarkOrange => new Color(1.0f, 0.5490196078431373f, 0.0f);

    /// <summary>
    /// DarkOrchid has an RGBA value of #9932CCFF (153, 50, 204, 255)
    /// </summary>
    public static Color DarkOrchid => new Color(0.6f, 0.19607843137254902f, 0.8f);

    /// <summary>
    /// DarkRed has an RGBA value of #8B0000FF (139, 0, 0, 255)
    /// </summary>
    public static Color DarkRed => new Color(0.5450980392156862f, 0.0f, 0.0f);

    /// <summary>
    /// DarkSalmon has an RGBA value of #E9967AFF (233, 150, 122, 255)
    /// </summary>
    public static Color DarkSalmon => new Color(0.9137254901960784f, 0.5882352941176471f, 0.47843137254901963f);

    /// <summary>
    /// DarkSeaGreen has an RGBA value of #8FBC8FFF (143, 188, 143, 255)
    /// </summary>
    public static Color DarkSeaGreen => new Color(0.5607843137254902f, 0.7372549019607844f, 0.5607843137254902f);

    /// <summary>
    /// DarkSlateBlue has an RGBA value of #483D8BFF (72, 61, 139, 255)
    /// </summary>
    public static Color DarkSlateBlue => new Color(0.2823529411764706f, 0.23921568627450981f, 0.5450980392156862f);

    /// <summary>
    /// DarkSlateGray has an RGBA value of #2F4F4FFF (47, 79, 79, 255)
    /// </summary>
    public static Color DarkSlateGray => new Color(0.1843137254901961f, 0.30980392156862746f, 0.30980392156862746f);

    /// <summary>
    /// DarkSlateGrey has an RGBA value of #2F4F4FFF (47, 79, 79, 255)
    /// </summary>
    public static Color DarkSlateGrey => new Color(0.1843137254901961f, 0.30980392156862746f, 0.30980392156862746f);

    /// <summary>
    /// DarkTurquoise has an RGBA value of #00CED1FF (0, 206, 209, 255)
    /// </summary>
    public static Color DarkTurquoise => new Color(0.0f, 0.807843137254902f, 0.8196078431372549f);

    /// <summary>
    /// DarkViolet has an RGBA value of #9400D3FF (148, 0, 211, 255)
    /// </summary>
    public static Color DarkViolet => new Color(0.5803921568627451f, 0.0f, 0.8274509803921568f);

    /// <summary>
    /// DeepPink has an RGBA value of #FF1493FF (255, 20, 147, 255)
    /// </summary>
    public static Color DeepPink => new Color(1.0f, 0.0784313725490196f, 0.5764705882352941f);

    /// <summary>
    /// DeepSkyBlue has an RGBA value of #00BFFFFF (0, 191, 255, 255)
    /// </summary>
    public static Color DeepSkyBlue => new Color(0.0f, 0.7490196078431373f, 1.0f);

    /// <summary>
    /// DimGray has an RGBA value of #696969FF (105, 105, 105, 255)
    /// </summary>
    public static Color DimGray => new Color(0.4117647058823529f, 0.4117647058823529f, 0.4117647058823529f);

    /// <summary>
    /// DimGrey has an RGBA value of #696969FF (105, 105, 105, 255)
    /// </summary>
    public static Color DimGrey => new Color(0.4117647058823529f, 0.4117647058823529f, 0.4117647058823529f);

    /// <summary>
    /// DodgerBlue has an RGBA value of #1E90FFFF (30, 144, 255, 255)
    /// </summary>
    public static Color DodgerBlue => new Color(0.11764705882352941f, 0.5647058823529412f, 1.0f);

    /// <summary>
    /// FireBrick has an RGBA value of #B22222FF (178, 34, 34, 255)
    /// </summary>
    public static Color FireBrick => new Color(0.6980392156862745f, 0.13333333333333333f, 0.13333333333333333f);

    /// <summary>
    /// FloralWhite has an RGBA value of #FFFAF0FF (255, 250, 240, 255)
    /// </summary>
    public static Color FloralWhite => new Color(1.0f, 0.9803921568627451f, 0.9411764705882353f);

    /// <summary>
    /// ForestGreen has an RGBA value of #228B22FF (34, 139, 34, 255)
    /// </summary>
    public static Color ForestGreen => new Color(0.13333333333333333f, 0.5450980392156862f, 0.13333333333333333f);

    /// <summary>
    /// Fuchsia has an RGBA value of #FF00FFFF (255, 0, 255, 255)
    /// </summary>
    public static Color Fuchsia => new Color(1.0f, 0.0f, 1.0f);

    /// <summary>
    /// Gainsboro has an RGBA value of #DCDCDCFF (220, 220, 220, 255)
    /// </summary>
    public static Color Gainsboro => new Color(0.8627450980392157f, 0.8627450980392157f, 0.8627450980392157f);

    /// <summary>
    /// GhostWhite has an RGBA value of #F8F8FFFF (248, 248, 255, 255)
    /// </summary>
    public static Color GhostWhite => new Color(0.9725490196078431f, 0.9725490196078431f, 1.0f);

    /// <summary>
    /// Gold has an RGBA value of #FFD700FF (255, 215, 0, 255)
    /// </summary>
    public static Color Gold => new Color(1.0f, 0.8431372549019608f, 0.0f);

    /// <summary>
    /// GoldenRod has an RGBA value of #DAA520FF (218, 165, 32, 255)
    /// </summary>
    public static Color GoldenRod => new Color(0.8549019607843137f, 0.6470588235294118f, 0.12549019607843137f);

    /// <summary>
    /// Gray has an RGBA value of #808080FF (128, 128, 128, 255)
    /// </summary>
    public static Color Gray => new Color(0.5019607843137255f, 0.5019607843137255f, 0.5019607843137255f);

    /// <summary>
    /// Grey has an RGBA value of #808080FF (128, 128, 128, 255)
    /// </summary>
    public static Color Grey => new Color(0.5019607843137255f, 0.5019607843137255f, 0.5019607843137255f);

    /// <summary>
    /// Green has an RGBA value of #008000FF (0, 128, 0, 255)
    /// </summary>
    public static Color Green => new Color(0.0f, 0.5019607843137255f, 0.0f);

    /// <summary>
    /// GreenYellow has an RGBA value of #ADFF2FFF (173, 255, 47, 255)
    /// </summary>
    public static Color GreenYellow => new Color(0.6784313725490196f, 1.0f, 0.1843137254901961f);

    /// <summary>
    /// HoneyDew has an RGBA value of #F0FFF0FF (240, 255, 240, 255)
    /// </summary>
    public static Color HoneyDew => new Color(0.9411764705882353f, 1.0f, 0.9411764705882353f);

    /// <summary>
    /// HotPink has an RGBA value of #FF69B4FF (255, 105, 180, 255)
    /// </summary>
    public static Color HotPink => new Color(1.0f, 0.4117647058823529f, 0.7058823529411765f);

    /// <summary>
    /// IndianRed has an RGBA value of #CD5C5CFF (205, 92, 92, 255)
    /// </summary>
    public static Color IndianRed => new Color(0.803921568627451f, 0.3607843137254902f, 0.3607843137254902f);

    /// <summary>
    /// Indigo has an RGBA value of #4B0082FF (75, 0, 130, 255)
    /// </summary>
    public static Color Indigo => new Color(0.29411764705882354f, 0.0f, 0.5098039215686274f);

    /// <summary>
    /// Ivory has an RGBA value of #FFFFF0FF (255, 255, 240, 255)
    /// </summary>
    public static Color Ivory => new Color(1.0f, 1.0f, 0.9411764705882353f);

    /// <summary>
    /// Khaki has an RGBA value of #F0E68CFF (240, 230, 140, 255)
    /// </summary>
    public static Color Khaki => new Color(0.9411764705882353f, 0.9019607843137255f, 0.5490196078431373f);

    /// <summary>
    /// Lavender has an RGBA value of #E6E6FAFF (230, 230, 250, 255)
    /// </summary>
    public static Color Lavender => new Color(0.9019607843137255f, 0.9019607843137255f, 0.9803921568627451f);

    /// <summary>
    /// LavenderBlush has an RGBA value of #FFF0F5FF (255, 240, 245, 255)
    /// </summary>
    public static Color LavenderBlush => new Color(1.0f, 0.9411764705882353f, 0.9607843137254902f);

    /// <summary>
    /// LawnGreen has an RGBA value of #7CFC00FF (124, 252, 0, 255)
    /// </summary>
    public static Color LawnGreen => new Color(0.48627450980392156f, 0.9882352941176471f, 0.0f);

    /// <summary>
    /// LemonChiffon has an RGBA value of #FFFACDFF (255, 250, 205, 255)
    /// </summary>
    public static Color LemonChiffon => new Color(1.0f, 0.9803921568627451f, 0.803921568627451f);

    /// <summary>
    /// LightBlue has an RGBA value of #ADD8E6FF (173, 216, 230, 255)
    /// </summary>
    public static Color LightBlue => new Color(0.6784313725490196f, 0.8470588235294118f, 0.9019607843137255f);

    /// <summary>
    /// LightCoral has an RGBA value of #F08080FF (240, 128, 128, 255)
    /// </summary>
    public static Color LightCoral => new Color(0.9411764705882353f, 0.5019607843137255f, 0.5019607843137255f);

    /// <summary>
    /// LightCyan has an RGBA value of #E0FFFFFF (224, 255, 255, 255)
    /// </summary>
    public static Color LightCyan => new Color(0.8784313725490196f, 1.0f, 1.0f);

    /// <summary>
    /// LightGoldenRodYellow has an RGBA value of #FAFAD2FF (250, 250, 210, 255)
    /// </summary>
    public static Color LightGoldenRodYellow => new Color(0.9803921568627451f, 0.9803921568627451f, 0.8235294117647058f);

    /// <summary>
    /// LightGray has an RGBA value of #D3D3D3FF (211, 211, 211, 255)
    /// </summary>
    public static Color LightGray => new Color(0.8274509803921568f, 0.8274509803921568f, 0.8274509803921568f);

    /// <summary>
    /// LightGrey has an RGBA value of #D3D3D3FF (211, 211, 211, 255)
    /// </summary>
    public static Color LightGrey => new Color(0.8274509803921568f, 0.8274509803921568f, 0.8274509803921568f);

    /// <summary>
    /// LightGreen has an RGBA value of #90EE90FF (144, 238, 144, 255)
    /// </summary>
    public static Color LightGreen => new Color(0.5647058823529412f, 0.9333333333333333f, 0.5647058823529412f);

    /// <summary>
    /// LightPink has an RGBA value of #FFB6C1FF (255, 182, 193, 255)
    /// </summary>
    public static Color LightPink => new Color(1.0f, 0.7137254901960784f, 0.7568627450980392f);

    /// <summary>
    /// LightSalmon has an RGBA value of #FFA07AFF (255, 160, 122, 255)
    /// </summary>
    public static Color LightSalmon => new Color(1.0f, 0.6274509803921569f, 0.47843137254901963f);

    /// <summary>
    /// LightSeaGreen has an RGBA value of #20B2AAFF (32, 178, 170, 255)
    /// </summary>
    public static Color LightSeaGreen => new Color(0.12549019607843137f, 0.6980392156862745f, 0.6666666666666666f);

    /// <summary>
    /// LightSkyBlue has an RGBA value of #87CEFAFF (135, 206, 250, 255)
    /// </summary>
    public static Color LightSkyBlue => new Color(0.5294117647058824f, 0.807843137254902f, 0.9803921568627451f);

    /// <summary>
    /// LightSlateGray has an RGBA value of #778899FF (119, 136, 153, 255)
    /// </summary>
    public static Color LightSlateGray => new Color(0.4666666666666667f, 0.5333333333333333f, 0.6f);

    /// <summary>
    /// LightSlateGrey has an RGBA value of #778899FF (119, 136, 153, 255)
    /// </summary>
    public static Color LightSlateGrey => new Color(0.4666666666666667f, 0.5333333333333333f, 0.6f);

    /// <summary>
    /// LightSteelBlue has an RGBA value of #B0C4DEFF (176, 196, 222, 255)
    /// </summary>
    public static Color LightSteelBlue => new Color(0.6901960784313725f, 0.7686274509803922f, 0.8705882352941177f);

    /// <summary>
    /// LightYellow has an RGBA value of #FFFFE0FF (255, 255, 224, 255)
    /// </summary>
    public static Color LightYellow => new Color(1.0f, 1.0f, 0.8784313725490196f);

    /// <summary>
    /// Lime has an RGBA value of #00FF00FF (0, 255, 0, 255)
    /// </summary>
    public static Color Lime => new Color(0.0f, 1.0f, 0.0f);

    /// <summary>
    /// LimeGreen has an RGBA value of #32CD32FF (50, 205, 50, 255)
    /// </summary>
    public static Color LimeGreen => new Color(0.19607843137254902f, 0.803921568627451f, 0.19607843137254902f);

    /// <summary>
    /// Linen has an RGBA value of #FAF0E6FF (250, 240, 230, 255)
    /// </summary>
    public static Color Linen => new Color(0.9803921568627451f, 0.9411764705882353f, 0.9019607843137255f);

    /// <summary>
    /// Magenta has an RGBA value of #FF00FFFF (255, 0, 255, 255)
    /// </summary>
    public static Color Magenta => new Color(1.0f, 0.0f, 1.0f);

    /// <summary>
    /// Maroon has an RGBA value of #800000FF (128, 0, 0, 255)
    /// </summary>
    public static Color Maroon => new Color(0.5019607843137255f, 0.0f, 0.0f);

    /// <summary>
    /// MediumAquaMarine has an RGBA value of #66CDAAFF (102, 205, 170, 255)
    /// </summary>
    public static Color MediumAquaMarine => new Color(0.4f, 0.803921568627451f, 0.6666666666666666f);

    /// <summary>
    /// MediumBlue has an RGBA value of #0000CDFF (0, 0, 205, 255)
    /// </summary>
    public static Color MediumBlue => new Color(0.0f, 0.0f, 0.803921568627451f);

    /// <summary>
    /// MediumOrchid has an RGBA value of #BA55D3FF (186, 85, 211, 255)
    /// </summary>
    public static Color MediumOrchid => new Color(0.7294117647058823f, 0.3333333333333333f, 0.8274509803921568f);

    /// <summary>
    /// MediumPurple has an RGBA value of #9370DBFF (147, 112, 219, 255)
    /// </summary>
    public static Color MediumPurple => new Color(0.5764705882352941f, 0.4392156862745098f, 0.8588235294117647f);

    /// <summary>
    /// MediumSeaGreen has an RGBA value of #3CB371FF (60, 179, 113, 255)
    /// </summary>
    public static Color MediumSeaGreen => new Color(0.23529411764705882f, 0.7019607843137254f, 0.44313725490196076f);

    /// <summary>
    /// MediumSlateBlue has an RGBA value of #7B68EEFF (123, 104, 238, 255)
    /// </summary>
    public static Color MediumSlateBlue => new Color(0.4823529411764706f, 0.40784313725490196f, 0.9333333333333333f);

    /// <summary>
    /// MediumSpringGreen has an RGBA value of #00FA9AFF (0, 250, 154, 255)
    /// </summary>
    public static Color MediumSpringGreen => new Color(0.0f, 0.9803921568627451f, 0.6039215686274509f);

    /// <summary>
    /// MediumTurquoise has an RGBA value of #48D1CCFF (72, 209, 204, 255)
    /// </summary>
    public static Color MediumTurquoise => new Color(0.2823529411764706f, 0.8196078431372549f, 0.8f);

    /// <summary>
    /// MediumVioletRed has an RGBA value of #C71585FF (199, 21, 133, 255)
    /// </summary>
    public static Color MediumVioletRed => new Color(0.7803921568627451f, 0.08235294117647059f, 0.5215686274509804f);

    /// <summary>
    /// MidnightBlue has an RGBA value of #191970FF (25, 25, 112, 255)
    /// </summary>
    public static Color MidnightBlue => new Color(0.09803921568627451f, 0.09803921568627451f, 0.4392156862745098f);

    /// <summary>
    /// MintCream has an RGBA value of #F5FFFAFF (245, 255, 250, 255)
    /// </summary>
    public static Color MintCream => new Color(0.9607843137254902f, 1.0f, 0.9803921568627451f);

    /// <summary>
    /// MistyRose has an RGBA value of #FFE4E1FF (255, 228, 225, 255)
    /// </summary>
    public static Color MistyRose => new Color(1.0f, 0.8941176470588236f, 0.8823529411764706f);

    /// <summary>
    /// Moccasin has an RGBA value of #FFE4B5FF (255, 228, 181, 255)
    /// </summary>
    public static Color Moccasin => new Color(1.0f, 0.8941176470588236f, 0.7098039215686275f);

    /// <summary>
    /// NavajoWhite has an RGBA value of #FFDEADFF (255, 222, 173, 255)
    /// </summary>
    public static Color NavajoWhite => new Color(1.0f, 0.8705882352941177f, 0.6784313725490196f);

    /// <summary>
    /// Navy has an RGBA value of #000080FF (0, 0, 128, 255)
    /// </summary>
    public static Color Navy => new Color(0.0f, 0.0f, 0.5019607843137255f);

    /// <summary>
    /// OldLace has an RGBA value of #FDF5E6FF (253, 245, 230, 255)
    /// </summary>
    public static Color OldLace => new Color(0.9921568627450981f, 0.9607843137254902f, 0.9019607843137255f);

    /// <summary>
    /// Olive has an RGBA value of #808000FF (128, 128, 0, 255)
    /// </summary>
    public static Color Olive => new Color(0.5019607843137255f, 0.5019607843137255f, 0.0f);

    /// <summary>
    /// OliveDrab has an RGBA value of #6B8E23FF (107, 142, 35, 255)
    /// </summary>
    public static Color OliveDrab => new Color(0.4196078431372549f, 0.5568627450980392f, 0.13725490196078433f);

    /// <summary>
    /// Orange has an RGBA value of #FFA500FF (255, 165, 0, 255)
    /// </summary>
    public static Color Orange => new Color(1.0f, 0.6470588235294118f, 0.0f);

    /// <summary>
    /// OrangeRed has an RGBA value of #FF4500FF (255, 69, 0, 255)
    /// </summary>
    public static Color OrangeRed => new Color(1.0f, 0.27058823529411763f, 0.0f);

    /// <summary>
    /// Orchid has an RGBA value of #DA70D6FF (218, 112, 214, 255)
    /// </summary>
    public static Color Orchid => new Color(0.8549019607843137f, 0.4392156862745098f, 0.8392156862745098f);

    /// <summary>
    /// PaleGoldenRod has an RGBA value of #EEE8AAFF (238, 232, 170, 255)
    /// </summary>
    public static Color PaleGoldenRod => new Color(0.9333333333333333f, 0.9098039215686274f, 0.6666666666666666f);

    /// <summary>
    /// PaleGreen has an RGBA value of #98FB98FF (152, 251, 152, 255)
    /// </summary>
    public static Color PaleGreen => new Color(0.596078431372549f, 0.984313725490196f, 0.596078431372549f);

    /// <summary>
    /// PaleTurquoise has an RGBA value of #AFEEEEFF (175, 238, 238, 255)
    /// </summary>
    public static Color PaleTurquoise => new Color(0.6862745098039216f, 0.9333333333333333f, 0.9333333333333333f);

    /// <summary>
    /// PaleVioletRed has an RGBA value of #DB7093FF (219, 112, 147, 255)
    /// </summary>
    public static Color PaleVioletRed => new Color(0.8588235294117647f, 0.4392156862745098f, 0.5764705882352941f);

    /// <summary>
    /// PapayaWhip has an RGBA value of #FFEFD5FF (255, 239, 213, 255)
    /// </summary>
    public static Color PapayaWhip => new Color(1.0f, 0.9372549019607843f, 0.8352941176470589f);

    /// <summary>
    /// PeachPuff has an RGBA value of #FFDAB9FF (255, 218, 185, 255)
    /// </summary>
    public static Color PeachPuff => new Color(1.0f, 0.8549019607843137f, 0.7254901960784313f);

    /// <summary>
    /// Peru has an RGBA value of #CD853FFF (205, 133, 63, 255)
    /// </summary>
    public static Color Peru => new Color(0.803921568627451f, 0.5215686274509804f, 0.24705882352941178f);

    /// <summary>
    /// Pink has an RGBA value of #FFC0CBFF (255, 192, 203, 255)
    /// </summary>
    public static Color Pink => new Color(1.0f, 0.7529411764705882f, 0.796078431372549f);

    /// <summary>
    /// Plum has an RGBA value of #DDA0DDFF (221, 160, 221, 255)
    /// </summary>
    public static Color Plum => new Color(0.8666666666666667f, 0.6274509803921569f, 0.8666666666666667f);

    /// <summary>
    /// PowderBlue has an RGBA value of #B0E0E6FF (176, 224, 230, 255)
    /// </summary>
    public static Color PowderBlue => new Color(0.6901960784313725f, 0.8784313725490196f, 0.9019607843137255f);

    /// <summary>
    /// Purple has an RGBA value of #800080FF (128, 0, 128, 255)
    /// </summary>
    public static Color Purple => new Color(0.5019607843137255f, 0.0f, 0.5019607843137255f);

    /// <summary>
    /// RebeccaPurple has an RGBA value of #663399FF (102, 51, 153, 255)
    /// </summary>
    public static Color RebeccaPurple => new Color(0.4f, 0.2f, 0.6f);

    /// <summary>
    /// Red has an RGBA value of #FF0000FF (255, 0, 0, 255)
    /// </summary>
    public static Color Red => new Color(1.0f, 0.0f, 0.0f);

    /// <summary>
    /// RosyBrown has an RGBA value of #BC8F8FFF (188, 143, 143, 255)
    /// </summary>
    public static Color RosyBrown => new Color(0.7372549019607844f, 0.5607843137254902f, 0.5607843137254902f);

    /// <summary>
    /// RoyalBlue has an RGBA value of #4169E1FF (65, 105, 225, 255)
    /// </summary>
    public static Color RoyalBlue => new Color(0.2549019607843137f, 0.4117647058823529f, 0.8823529411764706f);

    /// <summary>
    /// SaddleBrown has an RGBA value of #8B4513FF (139, 69, 19, 255)
    /// </summary>
    public static Color SaddleBrown => new Color(0.5450980392156862f, 0.27058823529411763f, 0.07450980392156863f);

    /// <summary>
    /// Salmon has an RGBA value of #FA8072FF (250, 128, 114, 255)
    /// </summary>
    public static Color Salmon => new Color(0.9803921568627451f, 0.5019607843137255f, 0.4470588235294118f);

    /// <summary>
    /// SandyBrown has an RGBA value of #F4A460FF (244, 164, 96, 255)
    /// </summary>
    public static Color SandyBrown => new Color(0.9568627450980393f, 0.6431372549019608f, 0.3764705882352941f);

    /// <summary>
    /// SeaGreen has an RGBA value of #2E8B57FF (46, 139, 87, 255)
    /// </summary>
    public static Color SeaGreen => new Color(0.1803921568627451f, 0.5450980392156862f, 0.3411764705882353f);

    /// <summary>
    /// SeaShell has an RGBA value of #FFF5EEFF (255, 245, 238, 255)
    /// </summary>
    public static Color SeaShell => new Color(1.0f, 0.9607843137254902f, 0.9333333333333333f);

    /// <summary>
    /// Sienna has an RGBA value of #A0522DFF (160, 82, 45, 255)
    /// </summary>
    public static Color Sienna => new Color(0.6274509803921569f, 0.3215686274509804f, 0.17647058823529413f);

    /// <summary>
    /// Silver has an RGBA value of #C0C0C0FF (192, 192, 192, 255)
    /// </summary>
    public static Color Silver => new Color(0.7529411764705882f, 0.7529411764705882f, 0.7529411764705882f);

    /// <summary>
    /// SkyBlue has an RGBA value of #87CEEBFF (135, 206, 235, 255)
    /// </summary>
    public static Color SkyBlue => new Color(0.5294117647058824f, 0.807843137254902f, 0.9215686274509803f);

    /// <summary>
    /// SlateBlue has an RGBA value of #6A5ACDFF (106, 90, 205, 255)
    /// </summary>
    public static Color SlateBlue => new Color(0.41568627450980394f, 0.35294117647058826f, 0.803921568627451f);

    /// <summary>
    /// SlateGray has an RGBA value of #708090FF (112, 128, 144, 255)
    /// </summary>
    public static Color SlateGray => new Color(0.4392156862745098f, 0.5019607843137255f, 0.5647058823529412f);

    /// <summary>
    /// SlateGrey has an RGBA value of #708090FF (112, 128, 144, 255)
    /// </summary>
    public static Color SlateGrey => new Color(0.4392156862745098f, 0.5019607843137255f, 0.5647058823529412f);

    /// <summary>
    /// Snow has an RGBA value of #FFFAFAFF (255, 250, 250, 255)
    /// </summary>
    public static Color Snow => new Color(1.0f, 0.9803921568627451f, 0.9803921568627451f);

    /// <summary>
    /// SpringGreen has an RGBA value of #00FF7FFF (0, 255, 127, 255)
    /// </summary>
    public static Color SpringGreen => new Color(0.0f, 1.0f, 0.4980392156862745f);

    /// <summary>
    /// SteelBlue has an RGBA value of #4682B4FF (70, 130, 180, 255)
    /// </summary>
    public static Color SteelBlue => new Color(0.27450980392156865f, 0.5098039215686274f, 0.7058823529411765f);

    /// <summary>
    /// Tan has an RGBA value of #D2B48CFF (210, 180, 140, 255)
    /// </summary>
    public static Color Tan => new Color(0.8235294117647058f, 0.7058823529411765f, 0.5490196078431373f);

    /// <summary>
    /// Teal has an RGBA value of #008080FF (0, 128, 128, 255)
    /// </summary>
    public static Color Teal => new Color(0.0f, 0.5019607843137255f, 0.5019607843137255f);

    /// <summary>
    /// Thistle has an RGBA value of #D8BFD8FF (216, 191, 216, 255)
    /// </summary>
    public static Color Thistle => new Color(0.8470588235294118f, 0.7490196078431373f, 0.8470588235294118f);

    /// <summary>
    /// Tomato has an RGBA value of #FF6347FF (255, 99, 71, 255)
    /// </summary>
    public static Color Tomato => new Color(1.0f, 0.38823529411764707f, 0.2784313725490196f);

    /// <summary>
    /// Turquoise has an RGBA value of #40E0D0FF (64, 224, 208, 255)
    /// </summary>
    public static Color Turquoise => new Color(0.25098039215686274f, 0.8784313725490196f, 0.8156862745098039f);

    /// <summary>
    /// Violet has an RGBA value of #EE82EEFF (238, 130, 238, 255)
    /// </summary>
    public static Color Violet => new Color(0.9333333333333333f, 0.5098039215686274f, 0.9333333333333333f);

    /// <summary>
    /// Wheat has an RGBA value of #F5DEB3FF (245, 222, 179, 255)
    /// </summary>
    public static Color Wheat => new Color(0.9607843137254902f, 0.8705882352941177f, 0.7019607843137254f);

    /// <summary>
    /// White has an RGBA value of #FFFFFFFF (255, 255, 255, 255)
    /// </summary>
    public static Color White => new Color(1.0f, 1.0f, 1.0f);

    /// <summary>
    /// WhiteSmoke has an RGBA value of #F5F5F5FF (245, 245, 245, 255)
    /// </summary>
    public static Color WhiteSmoke => new Color(0.9607843137254902f, 0.9607843137254902f, 0.9607843137254902f);

    /// <summary>
    /// Yellow has an RGBA value of #FFFF00FF (255, 255, 0, 255)
    /// </summary>
    public static Color Yellow => new Color(1.0f, 1.0f, 0.0f);

    /// <summary>
    /// YellowGreen has an RGBA value of #9ACD32FF (154, 205, 50, 255)
    /// </summary>
    public static Color YellowGreen => new Color(0.6039215686274509f, 0.803921568627451f, 0.19607843137254902f);
}