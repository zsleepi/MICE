using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MICE.scripts.lib.AI
{
    public class WanderAI : INPCController
    {
        public NPC Owner { get; set; }

        private Vector2I _currentDirection = Vector2I.Zero;
        private int _turnsUntilChange = 0;

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
        public void HandleBump(bool didBump)
        {
            if (didBump)
            {
                Owner.AudioPlayer.Play();
            }
        }
    }
}
