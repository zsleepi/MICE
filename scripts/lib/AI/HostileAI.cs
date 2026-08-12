using Godot;
using System.Collections.Generic;

namespace MICE.scripts.lib.AI
{
    public class HostileAI(NPC owner) : INPCController
    {
        private NPC owner = owner;
        public IActor Target { get; set; }
        public Vector2I TargetCell;

        private Vector2I _currentDirection = Vector2I.Zero;
        private int _pathfindCooldown = 0;
        private Queue<Vector2I> _queuedMoves = new Queue<Vector2I>();
        private bool LastMovementBumped = false;

        public void TakeTurn(List<Tween> tweens, Grid grid, Room room)
        {
            TickTurn();
            var direction = DecideDirection(grid);

            tweens.Add(ResolveMove(direction, grid, room));
            FaceDirection(direction);
            owner.turnCooldown += 1 / owner.GetMoveSpeed();
        }

        public void Attach(NPC npc)
        {
            owner = npc;
        }

        public void TickTurn()
        {
            _pathfindCooldown--;
        }

        public void FaceDirection(Vector2I dir)
        {
            if (dir.X != 0) owner.Sprite.FlipH = dir.X > 0;
        }

        public void HandleBump(bool didBump)
        {
            if (didBump)
            {
                LastMovementBumped = true;
                owner.AudioPlayer.Play();
            }
            else
            {
                LastMovementBumped = false;
            }
        }

        private Vector2 CellToWorld(Vector2I cell, Room CurrentRoom) => CurrentRoom.Terrain.ToGlobal(CurrentRoom.Terrain.MapToLocal(cell));

        // resolves intent, then starts the tween (and returns it)
        private Tween ResolveMove(Vector2I dir, Grid _grid, Room room)
        {
            if (dir == Vector2I.Zero) return null;

            Vector2I from = owner.Cell;
            Vector2I to = from + dir;

            if (_grid.IsFree(to)) // freedom to do the movement
            {
                HandleBump(false);
                _grid.Move(from, to);
                owner.Cell = to;

                Tween t = owner.CreateTween();
                t.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
                t.TweenProperty(owner.Node, "global_position", CellToWorld(to, room), 0.11f);
                return t;
            }
            else // blocked: playing the bump!
            {
                HandleBump(true);
                Vector2 rest = CellToWorld(from, room);
                Vector2 nudge = rest + (Vector2)dir * 5f;
                Tween t = owner.CreateTween();

                // duration modifier kind of arbitrary... but i think this feels good
                t.TweenProperty(owner.Node, "global_position", nudge, 0.11f * 0.5);
                t.TweenProperty(owner.Node, "global_position", rest, 0.11f * 0.5);
                return t;
            }
        }

        public Vector2I DecideDirection(Grid grid)
        {
            if (_queuedMoves.Count == 0 || LastMovementBumped || _pathfindCooldown <= 0)
            {
                if (Target != null) { TargetCell = Target.Cell; }
                _queuedMoves = GridUtils.BreadthFirstSearch(owner.Cell, TargetCell, grid, 30000);
                _pathfindCooldown = (int)GD.RandRange(10, 15);
            }
            _currentDirection = _queuedMoves.Dequeue();
            return _currentDirection;
        }
    }
}
