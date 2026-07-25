using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.itemlogic
{
    public interface IEquippable
    {
        public string EquipSlot { get; set; }
    }
}
