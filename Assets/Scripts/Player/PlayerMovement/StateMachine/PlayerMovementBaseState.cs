using UnityEngine;

public abstract class PlayerMovementBaseState
{
    protected PlayerMovementController _controller;

    //public PlayerMovementBaseState(PlayerMovementController controller)
    //{
    //    _controller = controller;
    //}

    public abstract void EnterState(PlayerMovementStateMachine stateMachine);
    public abstract void UpdateState(PlayerMovementStateMachine stateMachine);
    public abstract void FixedUpdateState(PlayerMovementStateMachine stateMachine);
    public abstract void ExitState(PlayerMovementStateMachine stateMachine);
}
