using Godot;

namespace MICE.scripts.lib.AI
{
    public interface INPCController
    {
        public void Attach(NPC npc);
        void HandleBump(bool didBump);
        Vector2I DecideDirection(Grid grid);
        void TickTurn();
    }
}
