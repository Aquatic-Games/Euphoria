using System;
using System.Collections.Generic;
using Euphoria.Math;
using Pie.Text;

namespace Euphoria.Render.Text;

internal class FontFace : IDisposable
{
    private const int Padding = 2;
    
    private readonly Face _face;
    private readonly List<Texture> _textures;

    private readonly Dictionary<(char, int), FaceCharacter> _characters;

    private Vector2T<int> _currentPos;
    private int _currentTexture;
    
    public readonly Size<int> TextureSize;

    public int NumTextures => _textures.Count;
    
    public FontFace(string path, Size<int> textureSize)
    {
        _face = Font.FreeType.CreateFace(path);
        _textures = [new Texture((byte[]) null, TextureSize)];

        _characters = new Dictionary<(char, int), FaceCharacter>();

        TextureSize = textureSize;
    }

    public (Texture, Character) GetCharacter(char c, int size)
    {
        if (!_characters.TryGetValue((c, size), out FaceCharacter character))
        {
            Pie.Text.Character chr = _face.GetCharacter(c, (uint) size);

            Size<int> charSize = new Size<int>(chr.Width, chr.Height);

            if (_currentPos.X + charSize.Width >= TextureSize.Width ||
                _currentPos.Y + charSize.Height >= TextureSize.Height)
            {
                _textures.Add(new Texture((byte[]) null, TextureSize));
                _currentTexture++;
            }

            Texture current = _textures[_currentTexture];
            
            current.Update(_currentPos.X, _currentPos.Y, charSize.Width, charSize.Height, chr.Bitmap);

            _currentPos += new Vector2T<int>(charSize.Width, charSize.Height);
        }

        return (_textures[character.TextureIndex], character.Character);
    }

    public void Dispose()
    {
        foreach (Texture texture in _textures)
            texture.Dispose();
        
        _face.Dispose();
    }

    private readonly struct FaceCharacter
    {
        public readonly int TextureIndex;
        public readonly Character Character;

        public FaceCharacter(int textureIndex, Character character)
        {
            TextureIndex = textureIndex;
            Character = character;
        }
    }
}