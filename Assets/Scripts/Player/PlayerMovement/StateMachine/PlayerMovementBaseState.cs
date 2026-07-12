using UnityEngine;

public abstract class PlayerMovementBaseState
{
    public abstract void EnterState(PlayerMovementStateMachine stateMachine);
    public abstract void UpdateState(PlayerMovementStateMachine stateMachine);
    public abstract void FixedUpdateState(PlayerMovementStateMachine stateMachine);
    public abstract void ExitState(PlayerMovementStateMachine stateMachine);
}
