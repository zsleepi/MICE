using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.itemlogic
{
    public interface IItem
    {
        string Name { get; set; }
        string Description { get; set; }
        int Value { get; set; }
        bool Stackable { get; set; }
        Texture2D Sprite { get; set; } 
    }
}
