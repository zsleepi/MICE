using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class World : Node2D
{
    [Export] public Room CurrentRoom;
    [Export] public Player Player;

	[Export] public float BumpDistance = 8f; // pixels to nudge when bumping

	private readonly Grid _grid = new();
	private bool _busy; // true while a turn is animating: this is input cooldown

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
		foreach (Actor actor in CurrentRoom.GetActors())
		{
			_grid.Register(actor, actor.Cell);
		}
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_busy) return; // ignore input mid-turn

		Vector2I dir = Player.GetIntent();
		if (dir != Vector2I.Zero)
		{
			// handling h flipping
			switch (dir.X)
			{
				case 1:  Player.Sprite.FlipH = true;  break;
				case -1: Player.Sprite.FlipH = false; break;
				default: break;
			}

            // doing the turn even when waiting
            _ = ProcessTurnAsync(dir);
		}
	}

	private async Task ProcessTurnAsync(Vector2I playerDir)
	{
		_busy = true;

		var tweens = new List<Tween>();

		// player acts first
		tweens.Add(ResolveMove(Player, playerDir));

		// actors react afterward
		foreach (Actor actor in CurrentRoom.GetActors())
		{
			tweens.Add(ResolveMove(actor, actor.DecideDirection(_grid)));
		}

		// waiting for all animation tweens to finish together
		// this part took forever......
		async Task WaitFor(Tween t) => await ToSignal(t, Tween.SignalName.Finished);
		await Task.WhenAll(tweens.Where(t => t != null).Select(WaitFor));

		_busy = false; // update busy!!
	}

	// resolves intent, then starts the tween (and returns it)
	private Tween ResolveMove(IActor actor, Vector2I dir)
	{
		if (dir == Vector2I.Zero) return null;

		Vector2I from = actor.Cell;
		Vector2I to = from + dir;

		if (_grid.IsFree(to)) // freedom to do the movement
		{
			_grid.Move(from, to);
			actor.Cell = to;

			Tween t = CreateTween();
			t.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			t.TweenProperty(actor.Node, "global_position", CellToWorld(to), actor.StepDuration);
			return t;
		}
        else // blocked: playing the bump!
        {			
			Vector2 rest = CellToWorld(from);
			Vector2 nudge = rest + (Vector2)dir * BumpDistance;
			Tween t = CreateTween();

			// duration modifier kind of arbitrary... but i think this feels good
			t.TweenProperty(actor.Node, "global_position", nudge, actor.StepDuration * 0.5f);
			t.TweenProperty(actor.Node, "global_position", rest, actor.StepDuration * 0.5f);
			return t;
		}
	}

	private Vector2 CellToWorld(Vector2I cell) =>
		CurrentRoom.Terrain.ToGlobal(CurrentRoom.Terrain.MapToLocal(cell));
}
