using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public class Weapon : Item, IEquippable
    {
        public int DamageValue { get; set; }
        public string EquipSlot { get; set; }
    }
}
