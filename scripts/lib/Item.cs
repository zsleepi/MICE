using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Item
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public int Value { get; set; }
        public bool Stackable { get; set; }
        [Export] public Sprite2D Sprite { get; set; }
    }
}
