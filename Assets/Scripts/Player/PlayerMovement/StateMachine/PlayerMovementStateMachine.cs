using UnityEngine;

public class PlayerMovementStateMachine
{
    public PlayerMovementController Controller { get; private set; }

    public PlayerIdleState IdleState { get; private set; }
    public PlayerWalkState WalkState { get; private set; }
    public PlayerRunState RunState { get; private set; }
    public PlayerCrouchState CrouchState { get; private set; }
    public PlayerJumpState JumpState { get; private set; }
    public PlayerFallState FallState { get; private set; }
    PlayerMovementBaseState _currentState;

    public PlayerMovementStateMachine(PlayerMovementController controller)
    {
        Controller = controller;
        Initialize();
    }

    private void Initialize()
    {
        IdleState = new PlayerIdleState();
        WalkState = new PlayerWalkState();
        RunState = new PlayerRunState();
        CrouchState = new PlayerCrouchState();
        JumpState = new PlayerJumpState();
        FallState = new PlayerFallState();

        _currentState = IdleState;
        _currentState.EnterState(this);
    }

    public void UpdateStateMachine()
    {
        _currentState.UpdateState(this);
    }

    public void FixedUpdateStateMachine()
    {
        _currentState.FixedUpdateState(this);
    }

    public void ChangeState(PlayerMovementBaseState newState)
    {
        _currentState.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }
}
