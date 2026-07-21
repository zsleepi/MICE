using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class CursedItem : Item
    {
        public CursedItem(string name, string description, int value, bool stackable) : base(name, description, value, stackable)
        {
        }
    }
}
