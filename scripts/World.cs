using Godot;
using MICE.scripts.lib.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

public partial class World : Node2D
{
    [Export] public Room CurrentRoom;
    [Export] public Player Player;

	[Export] public float BumpDistance = 5f; // pixels to nudge when bumping
	public float StepDuration = 0.11f;

    private readonly Grid _grid = new();
	private bool _TurnInProgress; // true while a turn is animating: this is input cooldown

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		foreach (Vector2I cell in CurrentRoom.Terrain.GetUsedCells())
		{
			TileData data = CurrentRoom.Terrain.GetCellTileData(cell);
            if (data != null && !data.GetCustomData("Walkable").AsBool())
            {
				_grid.SetImpassable(cell);
            }
        }

		// place the player at room's spawn cell
		Player.Cell = CurrentRoom.PlayerSpawnCell;
		Player.GlobalPosition = CellToWorld(Player.Cell);
		_grid.Register(Player, Player.Cell);

		// register actors placed in room as well
		foreach (NPC npc in CurrentRoom.GetActors())
		{
			_grid.Register(npc, npc.Cell);
            npc.GlobalPosition = CellToWorld(npc.Cell);

			// TODO: temp thing to see if this works
			if (npc.ai is HostileAI spider)
			{
				spider.Target = Player;
			}
        }
	}

	// attempting rewrite of turn system
	public void ProcessNPCTurns(float time, List<Tween> tweens)
	{
        // pass time depending on player action speed. increment actor turn cooldowns
        foreach (NPC npc in CurrentRoom.GetActors())
        {
            npc.TickTime(time);
        }

		// actors take turns
		bool turnsRemaining;
		do {
            turnsRemaining = false;
            foreach (NPC npc in CurrentRoom.GetActors().OrderBy(a => a.turnCooldown)) {
				if (npc.turnCooldown <= 0) { npc.ai.TakeTurn(tweens, _grid, CurrentRoom); } // ideally we wouldnt need to pass all this stuff down
                if (npc.turnCooldown <= 0) { turnsRemaining = true; }
            }
		} while (turnsRemaining);
    }

	// resolves intent, then starts the tween (and returns it)
	public Tween ResolveMove(IActor actor, Vector2I dir)
	{
		if (dir == Vector2I.Zero) return null;

		Vector2I from = actor.Cell;
		Vector2I to = from + dir;

		if (_grid.IsFree(to)) // freedom to do the movement
		{
            actor.HandleBump(false);
            _grid.Move(from, to);
			actor.Cell = to;

			Tween t = CreateTween();
			t.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			t.TweenProperty(actor.Node, "global_position", CellToWorld(to), StepDuration);
			return t;
		}
        else // blocked: playing the bump!
        {
            actor.HandleBump(true);
            Vector2 rest = CellToWorld(from);
			Vector2 nudge = rest + (Vector2)dir * BumpDistance;
			Tween t = CreateTween();

			// duration modifier kind of arbitrary... but i think this feels good
			t.TweenProperty(actor.Node, "global_position", nudge, StepDuration * 0.5);
			t.TweenProperty(actor.Node, "global_position", rest, StepDuration * 0.5);
			return t;
		}
	}

	private Vector2 CellToWorld(Vector2I cell) =>
		CurrentRoom.Terrain.ToGlobal(CurrentRoom.Terrain.MapToLocal(cell));
}
