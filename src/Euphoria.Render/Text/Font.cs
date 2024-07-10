using System;
using System.Numerics;
using Euphoria.Math;
using Euphoria.Render.Renderers;
// TODO: grabs.Text!
using Pie.Text;

namespace Euphoria.Render.Text;

public class Font : IDisposable
{
    private FontFace _face;
    
    public Font(string path)
    {
        _face = new FontFace(path, new Size<int>(2048, 1024));
    }

    public void Draw(TextureBatcher batcher, Vector2 position, string text, int size, Color color)
    {
        Vector2 currentPos = position;

        int maxBearing = 0;
        foreach (char c in text)
        {
            (Texture texture, Character character) = _face.GetCharacter(c, size);
            if (character.Bearing.Y >= maxBearing)
                maxBearing = character.Bearing.Y;
        }

        currentPos.Y += maxBearing;
        
        foreach (char c in text)
        {
            (Texture texture, Character character) = _face.GetCharacter(c, size);
            
            switch (c)
            {
                case ' ':
                    currentPos.X += character.Advance;
                    continue;
                case '\n':
                    currentPos.X = position.X;
                    currentPos.Y += size;
                    continue;
            }

            Vector2 pos = currentPos + new Vector2(character.Bearing.X, -character.Bearing.Y);
            
            batcher.Draw(texture, pos, color, source: new Rectangle<int>(character.TexPosition, character.Size));

            currentPos.X += character.Advance;
        }
    }
    
    public void Dispose()
    {
        _face.Dispose();
    }

    internal static FreeType FreeType;

    static Font()
    {
        FreeType = new FreeType();
    }
}