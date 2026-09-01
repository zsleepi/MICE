using Godot;
using MICE.scripts.lib;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class TurnManager : Node
{
    [Export] World World;
    [Export] PlayerSightManager PlayerSight;

    [Signal] public delegate void TurnStartedEventHandler(int turnNumber);
    [Signal] public delegate void TurnCompletedEventHandler(int turnNumber);
    [Signal] public delegate void PlayerActedEventHandler(Vector2I direction);
    public bool IsTurnInProgress() => _turnInProgress;

    private bool _turnInProgress;
    private int _turnNumber = 0;

    public void TryDoTurn(Vector2I dir)
    {
        if (_turnInProgress) return;

        World.Player.FaceDirection(dir);

        if (dir != Vector2I.Zero)
        {
            _ = SafeProcessTurn(dir);
        }
    }
    private async Task SafeProcessTurn(Vector2I dir)
    {
        try
        {
            await ProcessTurnAsync(dir);
        }
        catch (System.Exception e)
        {
            GD.PrintErr($"[TurnManager] Turn threw: {e}");
        }
        finally
        {
            // always release the lock, even if something exploded
            _turnInProgress = false;
        }
    }

    private async Task ProcessTurnAsync(Vector2I playerDir)
    {
        _turnInProgress = true;
        _turnNumber++;
        EmitSignal(SignalName.TurnStarted, _turnNumber);

        var tweens = new List<Tween>();

        // player acts first
        tweens.Add(World.ResolveMove(World.Player, playerDir));
        EmitSignal(SignalName.PlayerActed, playerDir);

        // no transition so NPC turns are normal. runs concurrently w/ user in the background
        float time = 1 / World.Player.GetMoveSpeed();
        World.ProcessNPCTurns(time, tweens);

        if (tweens[0] != null)
        {
            await ToSignal(tweens[0], Tween.SignalName.Finished);
        }

        // horizontal transition check here
        var transition = World.CurrentRoom.GetTransitionAt(World.Player.Cell);
        if (transition != null)
        {
            // player stepped on a transition so swap rooms instead of doing NPC turns
            await TransitionToRoom(transition);

            EmitSignal(SignalName.TurnCompleted, _turnNumber);
            return;
        }

        // wait for all remaining tweens (NPC moves) to finish
        async Task WaitFor(Tween t) => await ToSignal(t, Tween.SignalName.Finished);
        await Task.WhenAll(tweens.Skip(1).Where(t => t != null).Select(WaitFor));

        EmitSignal(SignalName.TurnCompleted, _turnNumber);
    }

    private async Task TransitionToRoom(RoomTransitionCell transition)
    {
        var newRoomScene = transition.LoadTargetRoom();
        if (newRoomScene == null)
        {
            GD.PrintErr($"TurnManager: Failed to load target room '{transition.TargetRoomPath}'");
            return;
        }

        var newRoom = newRoomScene.Instantiate<Room>();

        // triggers Room._Ready, caching spawn points and transitions
        World.AddChild(newRoom);

        Vector2I spawnCell;

        if (transition.IsVertical)
        {
            // find partner staircase in dest room
            var partner = newRoom.GetTransitionById(transition.TargetSpawnId);
            if (partner == null) // womp womp
            {
                GD.PrintErr($"[Transition] Partner staircase '{transition.TargetSpawnId}' not found in {newRoom.Name}");
                World.RemoveChild(newRoom);
                newRoom.QueueFree();
                return;
            }

            spawnCell = partner.TopLeftCell; // because vertical movement should be one tile only
        }
        else
        {
            // find the spawn point in the new room
            var spawn = newRoom.GetSpawnPoint(transition.TargetSpawnId);
            if (spawn == null)
            {
                GD.PrintErr($"TurnManager: Spawn point '{transition.TargetSpawnId}' not found in target room!");
                World.RemoveChild(newRoom);
                newRoom.QueueFree();
                return;
            }

            spawnCell = spawn.GetCell();
        }        

        // remove old room
        var oldRoom = World.CurrentRoom;
        World.RemoveChild(oldRoom);
        oldRoom.QueueFree();

        // swap the reference
        World.CurrentRoom = newRoom;

        // initialize new room stuff
        World.OnRoomLoaded(newRoom, spawnCell);

        // huhhhh
        await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
    }

    public void TryVerticalTransition(Vector2I direction)
    {
        if (_turnInProgress) return;

        var transition = World.CurrentRoom.GetVerticalTransitionAt(
            World.Player.Cell, direction);

        if (transition != null)
        {
            _ = SafeVerticalTransition(transition);
        }
    }

    private async Task SafeVerticalTransition(RoomTransitionCell transition)
    {
        try
        {
            _turnInProgress = true;
            _turnNumber++;
            EmitSignal(SignalName.TurnStarted, _turnNumber);

            var hopTween = CreateTween();
            hopTween.TweenProperty(World.Player, "scale",
                new Vector2(0.8f, 1.2f), 0.1f);
            hopTween.TweenProperty(World.Player, "scale",
                Vector2.One, 0.1f);
            await ToSignal(hopTween, Tween.SignalName.Finished);

            await TransitionToRoom(transition);

            EmitSignal(SignalName.TurnCompleted, _turnNumber);
        }
        catch (System.Exception e) // bla bla error handlingggg
        {
            GD.PrintErr($"[TurnManager] vertical transition threw: {e}");
        }
        finally
        {
            _turnInProgress = false;
        }
    }
}
