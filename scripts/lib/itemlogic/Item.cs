using Godot;
using MICE.scripts.lib.itemlogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public partial class Item(string name, string description, int value, bool stackable, Texture2D sprite) : Resource, IItem
    {
        [Export] public string Name { get; set; } = name;
        [Export] public string Description { get; set; } = description;
        [Export] public int Value { get; set; } = value;
        [Export] public bool Stackable { get; set; } = stackable;
        [Export] public Texture2D Sprite { get; set; } = sprite;
    }
}
