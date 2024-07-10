using Euphoria.Math;

namespace Euphoria.Render.Text;

public readonly struct Character
{
    public readonly Vector2T<int> TexPosition;
    
    public readonly Size<int> Size;

    public readonly Vector2T<int> Bearing;

    public readonly int Advance;

    public Character(Vector2T<int> texPosition, Size<int> size, Vector2T<int> bearing, int advance)
    {
        TexPosition = texPosition;
        Size = size;
        Bearing = bearing;
        Advance = advance;
    }
}