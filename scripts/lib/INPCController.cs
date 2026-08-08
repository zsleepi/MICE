using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib
{
    public partial interface INPCController
    {
        NPC Owner { get; set; }
        public void Attach(NPC npcReference)
        {
            Owner = npcReference;
        }

        // all NPC actors need this -- vector2I value of zero is "wait"
        abstract Vector2I DecideDirection(Grid grid);
        abstract void TickTurn();

        public void HandleBump(bool didBump);
    }
}
