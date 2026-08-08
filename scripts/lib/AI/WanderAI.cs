using Godot;
using System;

namespace MICE.scripts.lib.AI
{
    public class WanderAI : INPCController
    {
        private NPC _owner;
        private Vector2I _currentDirection = Vector2I.Zero;
        private int _turnsUntilChange = 0;

        public void Attach(NPC npc)
        {
            _owner = npc;
        }

        public void TickTurn()
        {
            _currentDirection = Vector2I.Zero;
            _turnsUntilChange--;
        }

        public Vector2I DecideDirection(Grid grid)
        {
            if (_turnsUntilChange > 0)
            {
                return _currentDirection;
            }

            _currentDirection = GetRandomDirection();
            _turnsUntilChange = (int)GD.RandRange(2, 6);
            return _currentDirection;
        }

        public void HandleBump(bool didBump)
        {
            // fox didn't override HandleBump, so this stays empty
        }

        private Vector2I GetRandomDirection()
        {
            uint x, y;

            do
            {
                x = GD.Randi() % 3 - 1;
                y = GD.Randi() % 3 - 1;
            } while (x == 0 && y == 0);

            return new Vector2I((int)x, (int)y);
        }

        // i know exactly what i'm doing
    }
}