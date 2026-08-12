using Godot;
using System;
using System.Collections.Generic;

namespace MICE.scripts.lib.AI
{
    public class WanderAI(NPC owner) : INPCController
    {
        private NPC owner = owner;
        private Vector2I _currentDirection = Vector2I.Zero;
        private int _turnsUntilChange = 0;

        public void TakeTurn(List<Tween> tweens, Grid grid, Room room)
        {
            var direction = DecideDirection(grid);
            TickTurn();
            tweens.Add(ResolveMove(direction, grid, room));
            FaceDirection(direction);
            owner.turnCooldown += 1 / owner.GetMoveSpeed();
        }

        private Vector2 CellToWorld(Vector2I cell, Room CurrentRoom) => CurrentRoom.Terrain.ToGlobal(CurrentRoom.Terrain.MapToLocal(cell));

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

        public void Attach(NPC npc)
        {
            owner = npc;
        }

        public void TickTurn()
        {
            _currentDirection = Vector2I.Zero;
            _turnsUntilChange--;
        }

        public void FaceDirection(Vector2I dir)
        {
            if (dir.X != 0) owner.Sprite.FlipH = dir.X > 0;
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
            if (didBump)
            {
                owner.AudioPlayer.Play();
            }
        }

        private Vector2I GetRandomDirection()
        {
            uint x, y;

            do {
                x = GD.Randi() % 3 - 1;
                y = GD.Randi() % 3 - 1;
            } while (x == 0 && y == 0);

            return new Vector2I((int)x, (int)y);
        }
    }
}