using System;
using System.Collections.Generic;
using System.Numerics;
using Euphoria.Core;
using Euphoria.Math;
using Euphoria.Render.Renderers;
// TODO: grabs.Text!
using Pie.Text;

namespace Euphoria.Render.Text;

public class Font : IDisposable
{
    private FontFace _face;

    public readonly ulong Id;
    
    public Font(string path, params string[] subFonts)
    {
        _face = new FontFace(path, new Size<int>(2048, 1024));
        
        foreach (string font in subFonts)
            _face.AddSubFace(font);

        Id = _loadedFonts.AddItem(this);
    }

    public void Draw(TextureBatcher batcher, Vector2 position, string text, int size, Color color)
    {
        Vector2 currentPos = position;

        int maxBearing = 0;
        foreach (char c in text)
        {
            (_, Character character) = _face.GetCharacter(c, size);
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

    public Size<int> MeasureString(string text, int size)
    {
        Size<int> currentSize = Size<int>.Zero;
        int currentX = 0;
        int currentY = 0;

        foreach (char c in text)
        {
            (_, Character character) = _face.GetCharacter(c, size);

            switch (c)
            {
                case '\n':
                    currentY += size;
                    currentX = 0;
                    break;
            }
            
            currentX += character.Advance;
            if (currentX >= currentSize.Width)
                currentSize.Width = currentX;

            if (character.Size.Height >= currentSize.Height - currentY)
                currentSize.Height = currentY + character.Size.Height;
        }

        return currentSize;
    }
    
    public void Dispose()
    {
        _face.Dispose();
        
        _loadedFonts.RemoveItem(Id);
    }
    
    private static ItemIdCollection<Font> _loadedFonts;
    private static Dictionary<string, ulong> _namedFonts;

    internal static FreeType FreeType;

    static Font()
    {
        FreeType = new FreeType();
        _loadedFonts = new ItemIdCollection<Font>();
        _namedFonts = new Dictionary<string, ulong>();
    }

    public static void StoreFont(string name, Font font)
    {
        _namedFonts[name] = font.Id;
    }

    public static Font GetFont(ulong id)
        => _loadedFonts[id];

    public static Font GetFont(string name)
        => _loadedFonts[_namedFonts[name]];

    public static void DisposeAllFonts()
    {
        foreach ((_, Font font) in _loadedFonts.Items)
            font.Dispose();
    }
}