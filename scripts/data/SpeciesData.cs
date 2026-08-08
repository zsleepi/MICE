using MICE.scripts.lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Godot;

namespace MICE.scripts.data
{
    public static class SpeciesData
    {
        public static List<Species> list = [
            new Species("spider", "spiders", new Signature("4F3D99", "FF0C91"), "spider", new CoreSkills(-1,0,2,1,0,0) ),
            new Species("mousefolk", "micefolk", new Signature("FCEFEA", "D35658"), "micefolk", new CoreSkills(1,-1,2,0,0,1) ),
            new Species("fox", "foxes", new Signature("EFB87C", "E5511B"), "fox", new CoreSkills(0,0,1,0,0,2) ),
            ];

        public static Species GetSpecies(string _name)
        {
            return list.Find(item => item.name == _name);
        }

        public static Texture2D GetCharSprite(string _species, int _variant, string _job)
        {
            string _speciesPath = GetSpecies(_species).spritePath;
            Texture2D _texture = (Texture2D)GD.Load("res://assets/sprites/chars/" + _speciesPath + ".png");
            return _texture;
        }
    };
}
