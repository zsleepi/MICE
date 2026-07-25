using Godot;
using MICE.scripts.lib.itemlogic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public partial class Weapon(string name, string description, int value, bool stackable, Texture2D sprite, int damageValue, string equipSlot) : Item(name, description, value, stackable, sprite), IEquippable, IItem
    {
        [Export] public int DamageValue { get; set; } = damageValue;
        [Export] public string EquipSlot { get; set; } = equipSlot;
    }
}
