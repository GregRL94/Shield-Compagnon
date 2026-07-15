using UnityEngine;

public class PlayerFallState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        Debug.Log("Entered Fall State");
        // Set the player's state to falling
        stateMachine.Controller.IsFalling = true;
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        // Check if the player has landed
        if (stateMachine.Controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {
        // Apply gravity to the player
        // stateMachine.Controller.ApplyGravity();
    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        Debug.Log("Exited Fall State");
        // Reset the player's falling state
        stateMachine.Controller.IsFalling = false;
    }
}
