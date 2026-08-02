using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    internal class SpriteUtils
    {
        // works for any sprite that uses 2 colors
        public static Texture2D RecolorSprite(Image _sprite, Signature _signature)
        {
            Image _recolor = _sprite;
            Color[] _oldSignature = [];
            int _width = _recolor.GetWidth();
            int _height = _recolor.GetHeight();
            for (int y = 0; y < _height; y++)
            {
                for (int x = 0; x < _width; x++)
                {
                    Color _pixelColor = _recolor.GetPixel(x, y);
                    if (!_oldSignature.Contains(_pixelColor))
                    {
                        _oldSignature.Append(_pixelColor);
                    }
                    for (int i = 0; i < _oldSignature.Length; i++)
                    {
                        if (_pixelColor == _oldSignature[i])
                        {
                            if (i ==0) {
                                _recolor.SetPixel(x,y,_signature.primary); } else
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
