using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class World : Node2D
{
    [Export] public Room CurrentRoom;
    [Export] public Player Player;
	[Export] public CameraController Camera;

	[Export] public float BumpDistance = 5f; // pixels to nudge when bumping

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

		Camera.SetBounds(CurrentRoom.GetPlayArea());

        // place the player at room's spawn cell
        Player.Cell = CurrentRoom.PlayerSpawnCell;
		Player.GlobalPosition = CellToWorld(Player.Cell);
		_grid.Register(Player, Player.Cell);

		// register actors placed in room as well
		foreach (Actor actor in CurrentRoom.GetActors())
		{
			_grid.Register(actor, actor.Cell);
            actor.GlobalPosition = CellToWorld(actor.Cell);

			// TODO: temp thing to see if this works
			if (actor is Spider spider)
			{
				spider.Target = Player;
			}
        }
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (_busy) return; // ignore input mid-turn

		Vector2I dir = Player.GetIntent();

		// handling h flipping
		Player.FaceDirection(dir);

        // doing the turn even when waiting
        if (dir != Vector2I.Zero)
		{
            _ = ProcessTurnSafely(dir);
		}
	}

	// new process scaffolding... will help us debug
	private async Task ProcessTurnSafely(Vector2I dir)
	{
		try
		{
			await ProcessTurnAsync(dir);
		}
		catch (Exception ex)
		{
			GD.PrintErr($"[Turn Error] {ex}");
			_busy = false;
		}
	}

	private async Task ProcessTurnAsync(Vector2I playerDir)
	{
		_busy = true;

		var tweens = new List<Tween>();

		// player acts first
		tweens.Add(ResolveMove(Player, playerDir));

		// NPC actors react afterward
		foreach (NPC actor in CurrentRoom.GetActors())
		{
			var actorDir = actor.DecideDirection(_grid);

			tweens.Add(ResolveMove(actor, actorDir));
			actor.FaceDirection(actorDir);
			actor.TickTurn();
		}

		// waiting for all animation tweens to finish together
		// this part took forever......
		async Task WaitFor(Tween t) => await ToSignal(t, Tween.SignalName.Finished);
		await Task.WhenAll(tweens.Where(t => t != null).Select(WaitFor));

		// check this at the end!
		await CheckRoomTransition();

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
            actor.HandleBump(false);
            _grid.Move(from, to);
			actor.Cell = to;

			Tween t = CreateTween();
			t.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			t.TweenProperty(actor.Node, "global_position", CellToWorld(to), actor.StepDuration);
			return t;
		}
        else // blocked: playing the bump!
        {
            actor.HandleBump(true);
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

	private async Task CheckRoomTransition()
	{
		foreach (var rt in CurrentRoom.GetRoomTransitions())
		{
			if (rt.Contains(Player.Cell))
			{
				await TransitionToRoom(rt);
				return;
			}
		}
	}

	private async Task TransitionToRoom(RoomTransitionCell rt)
	{
		var newRoom = rt.LoadTargetRoom().Instantiate<Room>();

		// free up old room and add new
		CurrentRoom.QueueFree();
		AddChild(newRoom);
		CurrentRoom = newRoom;

		_grid.Clear();

		foreach (Vector2I cell in CurrentRoom.Terrain.GetUsedCells())
		{
			TileData data = CurrentRoom.Terrain.GetCellTileData(cell);
			if (data != null && !data.GetCustomData("Walkable").AsBool())
			{
				_grid.SetImpassable(cell);
			}
		}

		// getting spawn cell based on matching id
        Vector2I spawnCell = string.IsNullOrEmpty(rt.TargetSpawnId)
			? CurrentRoom.PlayerSpawnCell
			: CurrentRoom.FindSpawnCell(rt.TargetSpawnId);

		// TODO: remove these debugs when im sure this is ok lol
        //GD.Print($"[Transition] TargetSpawnId: {rt.TargetSpawnId}");
        //GD.Print($"[Transition] SpawnCell: {spawnCell}");
        //GD.Print($"[Transition] CellToWorld: {CellToWorld(spawnCell)}");
        //GD.Print($"[Transition] Player going to: {CellToWorld(spawnCell)}");

        Player.Cell = spawnCell;
        Player.GlobalPosition = CellToWorld(Player.Cell);
        _grid.Register(Player, Player.Cell);

        foreach (Actor actor in CurrentRoom.GetActors())
        {
            _grid.Register(actor, actor.Cell);
            actor.GlobalPosition = CellToWorld(actor.Cell);

            // set up spider target for new room's spiders!!
            if (actor is Spider newSpider)
            {
                newSpider.Target = Player;
            }
        }

        // compute bounds from terrain cells and inform camera
        Rect2 playArea = CurrentRoom.GetPlayArea();
		Camera.SetBounds(playArea);
		Camera.Follow(Player); // TODO: for now! we can do fancy dynamic stuff later
		Camera.TeleportToTarget();
	}
}
