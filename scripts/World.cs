using Godot;
using MICE.scripts.lib;
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

    public readonly Grid _grid = new();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // first time setup to init starting room
        OnRoomLoaded(CurrentRoom, CurrentRoom.PlayerSpawnCell);
    }

    public void OnRoomLoaded(Room room, Vector2I playerSpawnCell)
    {
        _grid.Clear();

        foreach (Vector2I cell in room.Terrain.GetUsedCells())
        {
            TileData data = room.Terrain.GetCellTileData(cell);
            if (data != null && !data.GetCustomData("Walkable").AsBool())
            {
                _grid.SetImpassable(cell);
            }
            if (data != null && data.GetCustomData("ObstructsView").AsBool())
            {
                _grid.SetObstruction(cell);
            }
            if (data != null && data.GetCustomData("LightSource").AsBool())
            {
                _grid.SetLightSource(cell);
            }
        }
        foreach (Vector2I lightSource in _grid.GetLightSources())
        {
            List<Vector2I> litTiles = SightLogic.GetVisibleTiles(lightSource, 5, _grid, 5);
            foreach (Vector2I Coord in litTiles)
            {
                _grid.SetLit(Coord);
            }
        }

        Player.Cell = playerSpawnCell;
        Player.GlobalPosition = CellToWorld(playerSpawnCell);
        _grid.Register(Player, Player.Cell);

        foreach (NPC npc in room.GetActors())
        {
            _grid.Register(npc, npc.Cell);
            npc.GlobalPosition = CellToWorld(npc.Cell);

            if (npc.ai is HostileAI spider)
            {
                spider.Target = Player;
            }
        }

        UpdateCameraBounds(room);
    }

    private void UpdateCameraBounds(Room room)
    {
        var terrain = room.Terrain;
        Rect2I usedRect = terrain.GetUsedRect();
        Vector2 topLeft = terrain.ToGlobal(terrain.MapToLocal(usedRect.Position));
        Vector2 bottomRight = terrain.ToGlobal(terrain.MapToLocal(
            usedRect.Position + usedRect.Size));

        var camera = GetViewport().GetCamera2D();
        if (camera != null)
        {
            camera.LimitLeft = (int)topLeft.X;
            camera.LimitTop = (int)topLeft.Y;
            camera.LimitRight = (int)bottomRight.X;
            camera.LimitBottom = (int)bottomRight.Y - Grid.TILE_SIZE; // truncates bottom pt of tiles at bottom of screen (which is good)
        }
    }

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
