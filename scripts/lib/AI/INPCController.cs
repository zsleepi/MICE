using Godot;
using System.Collections.Generic;

namespace MICE.scripts.lib.AI
{
    public interface INPCController
    {
        public void TakeTurn(List<Tween> tweens, Room room);
        public void Attach(NPC npc);
        void HandleBump(bool didBump);
        Vector2I DecideDirection();
        void TickTurn();
    }
}
