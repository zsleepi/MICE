using Godot;
using MICE.scripts.lib.itemlogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public partial class CursedItem(string name, string description, int value, bool stackable, Texture2D sprite) : Item(name, description, value, stackable, sprite), IItem
    {
    }
}
