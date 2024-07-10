using System;
using System.Collections.Generic;
using Euphoria.Core;
using Euphoria.Math;
using Pie.Text;

namespace Euphoria.Render.Text;

internal class FontFace : IDisposable
{
    private const int Padding = 1;
    
    private readonly Face _face;
    private List<Face> _subFaces;
    private readonly List<Texture> _textures;

    private readonly Dictionary<(char, int), FaceCharacter> _characters;

    private Vector2T<int> _currentPos;
    private int _currentTexture;
    
    public readonly Size<int> TextureSize;

    public int NumTextures => _textures.Count;
    
    public FontFace(string path, Size<int> textureSize)
    {
        TextureSize = textureSize;
        
        _face = Font.FreeType.CreateFace(path);
        _textures = [new Texture((byte[]) null, TextureSize)];

        _characters = new Dictionary<(char, int), FaceCharacter>();

        _currentPos = new Vector2T<int>(Padding);
        _currentTexture = 0;
    }

    public void AddSubFace(string path)
    {
        _subFaces ??= new List<Face>();
        
        _subFaces.Add(Font.FreeType.CreateFace(path));
    }

    public (Texture, Character) GetCharacter(char c, int size)
    {
        if (!_characters.TryGetValue((c, size), out FaceCharacter character))
        {
            Face face = _face;

            if (!face.CharacterExists(c) && _subFaces != null)
            {
                for (int i = 0; i < _subFaces.Count; i++)
                {
                    if (_subFaces[i].CharacterExists(c))
                    {
                        face = _subFaces[i];
                        break;
                    }
                }
            }
            
            Pie.Text.Character chr = face.GetCharacter(c, (uint) size);

            Size<int> charSize = new Size<int>(chr.Width, chr.Height);

            if (_currentPos.X + charSize.Width + Padding >= TextureSize.Width)
            {
                _currentPos.X = Padding;
                _currentPos.Y += size + Padding;
                if (_currentPos.Y + size + Padding >= TextureSize.Height)
                {
                    _currentTexture++;
                    _currentPos = new Vector2T<int>(Padding);
                    Logger.Trace("Creating new font texture.");
                    _textures.Add(new Texture((byte[]) null, TextureSize));
                }
            }

            Texture current = _textures[_currentTexture];
            
            Logger.Trace($"Creating character '{c}' (size: {size}, bmp size: {charSize}, tex: {_currentTexture}, pos: {_currentPos}, max pos: {_currentPos + new Vector2T<int>(charSize.Width, charSize.Height)})");
            
            if (charSize != Size<int>.Zero)
                current.Update(_currentPos.X, _currentPos.Y, charSize.Width, charSize.Height, chr.Bitmap);

            character = new FaceCharacter(_currentTexture,
                new Character(_currentPos, charSize, new Vector2T<int>(chr.BitmapLeft, chr.BitmapTop), chr.Advance));
            _characters.Add((c, size), character);

            _currentPos.X += charSize.Width + Padding;
        }

        return (_textures[character.TextureIndex], character.Character);
    }

    public void Dispose()
    {
        foreach (Texture texture in _textures)
            texture.Dispose();

        if (_subFaces != null)
        {
            foreach (Face face in _subFaces)
                face.Dispose();
        }
        
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