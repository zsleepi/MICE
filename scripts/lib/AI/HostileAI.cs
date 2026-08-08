using Godot;
using System.Collections.Generic;

namespace MICE.scripts.lib.AI
{
    public class HostileAI : INPCController
    {
        public IActor Target { get; set; }
        public Vector2I TargetCell;

        private NPC _owner;
        private Vector2I _currentDirection = Vector2I.Zero;
        private int _moveCooldown = 0;
        private int _pathfindCooldown = 0;
        private Queue<Vector2I> _queuedMoves = new Queue<Vector2I>();
        private bool LastMovementBumped = false;

        public void Attach(NPC npc)
        {
            _owner = npc;
        }

        public void TickTurn()
        {
            _pathfindCooldown--;
            _moveCooldown--;
        }

        public void HandleBump(bool didBump)
        {
            if (didBump)
            {
                LastMovementBumped = true;
                _owner.AudioPlayer.Play();
            }
            else
            {
                LastMovementBumped = false;
            }
        }

        public Vector2I DecideDirection(Grid grid)
        {
            if (_moveCooldown > 0)
            {
                return Vector2I.Zero;
            }
            if (_queuedMoves.Count == 0 || LastMovementBumped || _pathfindCooldown <= 0)
            {
                if (Target != null) { TargetCell = Target.Cell; }
                _queuedMoves = GridUtils.BreadthFirstSearch(_owner.Cell, TargetCell, grid, 30000);
                _pathfindCooldown = (int)GD.RandRange(10, 15);
            }
            _currentDirection = _queuedMoves.Dequeue();
            _moveCooldown = 2;
            return _currentDirection;
        }
    }
}
