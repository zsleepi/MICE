using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

public partial class TurnManager : Node
{
    [Export] World World;

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
            _ = ProcessTurnAsync(dir);
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

        // did the player land on a transition cell?
        var transition = World.CurrentRoom.GetTransitionAt(World.Player.Cell);
        if (transition != null)
        {
            // player stepped on a transition so swap rooms instead of doing NPC turns
            await TransitionToRoom(transition);

            EmitSignal(SignalName.TurnCompleted, _turnNumber);
            _turnInProgress = false;
            return;
        }        

        // wait for all remaining tweens (NPC moves) to finish
        async Task WaitFor(Tween t) => await ToSignal(t, Tween.SignalName.Finished);
        await Task.WhenAll(tweens.Skip(1).Where(t => t != null).Select(WaitFor));

        EmitSignal(SignalName.TurnCompleted, _turnNumber);
        _turnInProgress = false;
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

        // find the spawn point in the new room
        var spawn = newRoom.GetSpawnPoint(transition.TargetSpawnId);
        if (spawn == null)
        {
            GD.PrintErr($"TurnManager: Spawn point '{transition.TargetSpawnId}' not found in target room!");
            World.RemoveChild(newRoom);
            newRoom.QueueFree();
            return;
        }

        // remove old room
        var oldRoom = World.CurrentRoom;
        World.RemoveChild(oldRoom);
        oldRoom.QueueFree();

        // swap the reference
        World.CurrentRoom = newRoom;

        // initialize new room stuff
        World.OnRoomLoaded(newRoom, spawn.GetCell());
    }
}
