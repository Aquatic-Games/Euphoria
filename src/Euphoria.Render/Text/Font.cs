using System;
using System.Collections.Generic;
using System.Numerics;
using System.Reflection;
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

    public Font(byte[] data)
    {
        _face = new FontFace(data, new Size<int>(2048, 1024));
        Id = _loadedFonts.AddItem(this);
    }

    public void Draw(TextureBatcher batcher, Vector2 position, string text, int size, Color color)
    {
        Vector2 currentPos = position;
        currentPos.Y += CalculateMaxBearing(text, size);
        
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

    public void DrawRichText(TextureBatcher batcher, Vector2 position, string text, int size, Color color)
    {
        Vector2 currentPos = position;
        currentPos.Y += CalculateMaxBearing(text, size);

        Color currentColor = color;
        int currentSize = size;
        
        for (int i = 0; i < text.Length; i++)
        {
            char c = text[i];
            
            (Texture texture, Character character) = _face.GetCharacter(c, currentSize);

            switch (c)
            {
                case ' ':
                    currentPos.X += character.Advance;
                    continue;
                case '\n':
                    currentPos.X = position.X;
                    currentPos.Y += size;
                    continue;
                
                case '<' when i > 0 && text[i - 1] != '\\':
                {
                    int textPos = i + 1;
                    while (text[i] != '>')
                        i++;

                    string argument = text[textPos..i].Trim();

                    if (argument.StartsWith('/'))
                    {
                        switch (argument)
                        {
                            case "/size":
                                currentSize = size;
                                break;
                            
                            case "/color":
                                currentColor = color;
                                break;
                            
                            default:
                                throw new Exception($"Unrecognized argument {argument}");
                        }
                    }
                    else
                    {
                        string[] splitArgument = argument.Split('=', StringSplitOptions.TrimEntries);

                        switch (splitArgument[0])
                        {
                            case "size":
                                currentSize = int.Parse(splitArgument[1]);
                                break;
                            
                            case "color":
                                currentColor = Color.FromString(splitArgument[1]);
                                break;
                        }
                    }

                    continue;
                }
            }
            
            Vector2 pos = currentPos + new Vector2(character.Bearing.X, -character.Bearing.Y);
            
            batcher.Draw(texture, pos, currentColor, source: new Rectangle<int>(character.TexPosition, character.Size));

            currentPos.X += character.Advance;
        }
    }

    public Size<int> MeasureString(string text, int size, MeasureMode mode = MeasureMode.LineHeight)
    {
        Size<int> currentSize = Size<int>.Zero;
        int currentX = 0;
        int currentY = 0;

        foreach (char c in text)
        {
            switch (c)
            {
                case '\n':
                    currentY += size;
                    currentX = 0;
                    continue;
            }
            
            (_, Character character) = _face.GetCharacter(c, size);
            
            currentX += character.Advance;
            if (currentX >= currentSize.Width)
                currentSize.Width = currentX;

            int totalHeight = mode switch
            {
                MeasureMode.LineHeight => character.Size.Height,
                MeasureMode.FullSize => character.Size.Height + (character.Size.Height - character.Bearing.Y),
                _ => throw new ArgumentOutOfRangeException(nameof(mode), mode, null)
            };
            
            if (totalHeight >= currentSize.Height - currentY)
                currentSize.Height = currentY + totalHeight;
        }

        return currentSize;
    }

    private int CalculateMaxBearing(string text, int size)
    {
        int maxBearing = 0;
        foreach (char c in text)
        {
            (_, Character character) = _face.GetCharacter(c, size);
            if (character.Bearing.Y >= maxBearing)
                maxBearing = character.Bearing.Y;
        }

        return maxBearing;
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
        
        Logger.Trace("Loading built-in fonts.");
        Roboto = new Font(Resource.LoadEmbedded("Euphoria.Render.Roboto-Regular.ttf", Assembly.GetExecutingAssembly()));
    }

    public static void StoreFont(string name, Font font)
    {
        Logger.Trace($"Assigning name \"{name}\" to font {font.Id}.");
        _namedFonts[name] = font.Id;
    }

    public static Font GetFont(ulong id)
        => _loadedFonts[id];

    public static Font GetFont(string name)
        => _loadedFonts[_namedFonts[name]];

    public static void DisposeAllFonts()
    {
        Logger.Debug("Disposing all fonts.");
        
        foreach ((_, Font font) in _loadedFonts.Items)
            font.Dispose();
        
        _loadedFonts.Items.Clear();
        _namedFonts.Clear();
    }

    public static readonly Font Roboto;
}