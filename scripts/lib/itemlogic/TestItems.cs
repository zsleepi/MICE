using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.itemlogic
{
    public class TestItems
    {

        private static Texture2D GetSprite(string _spriteName)
        {
            Texture2D _texture =  (Texture2D)GD.Load("res://assets/sprites/items/" + _spriteName + ".png");
            return _texture;
        }

        public static List<IItem> ItemList = [
        new Item("Stíck", "it's a stick", 1, true, GetSprite("stick")),
        new Item("Gold Nugget", "So shiny...", 100, true, GetSprite("gold_nugget")),
        new Weapon("Iron Sword", "swish swish swoosh", 50, false, GetSprite("metal_sword"), 12, "mainHand"),
        new Weapon("Wooden Sword", "swish swish swoosh but worse", 5, false, GetSprite("wooden_sword"), 5, "mainHand"),
        ];
    }
}
