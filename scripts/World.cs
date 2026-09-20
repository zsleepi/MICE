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

    [Signal] public delegate void RoomLoadedEventHandler();

    // Called when the node enters the scene tree for the first time.
    public override void _Ready()
    {
        // first time setup to init starting room
        OnRoomLoaded(CurrentRoom.PlayerSpawnCell);
    }

    public void OnRoomLoaded(Vector2I playerEntrance)
    {
        Grid.UpdateGridData(CurrentRoom);
        SetUpPlayer(playerEntrance);
        SetUpNPCs(CurrentRoom);
        EmitSignal(SignalName.RoomLoaded);
    }

    public async Task SwitchRoom(Room newRoom, Vector2I playerpos)
    {
        AddChild(newRoom);

        // remove old room
        var oldRoom = CurrentRoom;
        RemoveChild(oldRoom);
        oldRoom.QueueFree();

        // swap the reference
        CurrentRoom = newRoom;

        // initialize new room stuff
        OnRoomLoaded(playerpos);

        // huhhhh
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    public void SetUpPlayer(Vector2I _playerPos)
    {
        Player.Cell = _playerPos;
        Player.GlobalPosition = CellToWorld(_playerPos);
        Grid.Register(Player, Player.Cell);
    }

    public void SetUpNPCs(Room room)
    {
        foreach (NPC npc in room.GetActors())
        {
            Grid.Register(npc, npc.Cell);
            npc.GlobalPosition = CellToWorld(npc.Cell);

            if (npc.ai is HostileAI spider)
            {
                spider.Target = Player;
            }
        }
    }

    // TODO: move this to turnmanager
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
				if (npc.turnCooldown <= 0) { npc.ai.TakeTurn(tweens, CurrentRoom); } // ideally we wouldnt need to pass all this stuff down
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

		if (Grid.IsFree(to)) // freedom to do the movement
		{
            Grid.Move(from, to);
			actor.Cell = to;

			Tween t = CreateTween();
			t.SetTrans(Tween.TransitionType.Sine).SetEase(Tween.EaseType.Out);
			t.TweenProperty((Node2D)actor, "global_position", CellToWorld(to), StepDuration);
			return t;
		}
        else // blocked: playing the bump!
        {
            Vector2 rest = CellToWorld(from);
			Vector2 nudge = rest + (Vector2)dir * BumpDistance;
			Tween t = CreateTween();

			// duration modifier kind of arbitrary... but i think this feels good
			t.TweenProperty((Node2D)actor, "global_position", nudge, StepDuration * 0.5);
			t.TweenProperty((Node2D)actor, "global_position", rest, StepDuration * 0.5);
			return t;
		}
	}

	private Vector2 CellToWorld(Vector2I cell) =>
		CurrentRoom.Terrain.ToGlobal(CurrentRoom.Terrain.MapToLocal(cell));
}
