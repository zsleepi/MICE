using Godot;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

/// <summary>
/// Handles turns, turn taking, turn actions. Later on could send signals like TurnStarted or
/// TurnCompleted that other systems can listen to
/// </summary>
public partial class TurnManager : Node
{
    [Export] World World;

    private bool _turnInProgress;
    private int _turnNumber = 0;

    [Signal] public delegate void TurnStartedEventHandler(int turnNumber);
    [Signal] public delegate void TurnCompletedEventHandler(int turnNumber);
    [Signal] public delegate void PlayerActedEventHandler(Vector2I direction);

    public bool IsTurnInProgress() => _turnInProgress;

    public void TryDoTurn(Vector2I dir)
    {
        if (_turnInProgress) return;

        // handle visual facing immediately (snappy!)
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

        float time = 1 / World.Player.GetMoveSpeed();

        // npc acts next
        World.ProcessNPCTurns(time, tweens);

        // wait for all animation tweens to finish together
        async Task WaitFor(Tween t) => await ToSignal(t, Tween.SignalName.Finished);
        await Task.WhenAll(tweens.Where(t => t != null).Select(WaitFor));

        EmitSignal(SignalName.TurnCompleted, _turnNumber);
        _turnInProgress = false;
    }
}