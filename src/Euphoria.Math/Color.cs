namespace u4.Math;

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
}