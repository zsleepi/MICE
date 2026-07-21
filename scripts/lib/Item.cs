using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Item(string name, string description, int value, bool stackable)
    {
        public string Name { get; set; } = name;
        public string Description { get; set; } = description;
        public int Value { get; set; } = value;
        public bool Stackable { get; set; } = stackable;
        [Export] public Sprite2D Sprite { get; set; }
    }
}
