using Godot;
using System.Collections.Generic;

namespace MICE.scripts.lib.AI
{
    public interface INPCController
    {
        public void TakeTurn(List<Tween> tweens, Grid grid, Room room);
        public void Attach(NPC npc);
    }
}
