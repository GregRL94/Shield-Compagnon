using UnityEngine;

public class PlayerWalkState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        // Debug.Log("Entering Walk State");
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        if (stateMachine.Controller.MoveInput.magnitude == 0)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
        else if (stateMachine.Controller.IsSprinting)
        {
            stateMachine.ChangeState(stateMachine.RunState);
        }
        else if (stateMachine.Controller.IsCrouching)
        {
            stateMachine.ChangeState(stateMachine.CrouchState);
        }
        else if (stateMachine.Controller.IsJumping)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {
        
    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        
    }
}
