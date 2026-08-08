using Godot;
using MICE.scripts.data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    internal class SpriteUtils
    {
        public static Texture2D GetCharSprite(string species, string job, int variant)
        {
            Texture2D _texture = (Texture2D)GD.Load("res://assets/sprites/chars/" + SpeciesData.GetSpecies(species).SpritePath + ".png");
            return _texture;
        }


        // works for any sprite that uses 2 colors. has some problems though so rework all this later.
        public static Texture2D RecolorSprite(Image _sprite, Signature _signature)
        {
            Image _recolor = _sprite;
            List<Color> _oldSignature = [];
            int _width = _recolor.GetWidth();
            int _height = _recolor.GetHeight();
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Color _pixelColor = _recolor.GetPixel(x, y);
                    if (!_oldSignature.Contains(_pixelColor) && _pixelColor.A != 0 && _pixelColor.V != 0)
                    {
                        _oldSignature.Add(_pixelColor);
                    }
                    for (int i = 0; i < _oldSignature.Count; i++)
                    {
                        if (_pixelColor.Equals(_oldSignature[i]))
                        {
                            if (i == 0) {
                                _recolor.SetPixel(x ,y, _signature.primary); } else
                            {
                                _recolor.SetPixel(x, y, _signature.secondary);
                            }
                        }
                    }
                }
            }

            ImageTexture _newTexture = ImageTexture.CreateFromImage(_recolor);
            return _newTexture;
        }
    }
}
