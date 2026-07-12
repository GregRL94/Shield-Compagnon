using UnityEngine;

public class PlayerIdleState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (controller.IsJumping)
        {
            stateMachine.ChangeState(stateMachine.JumpState);
        }
        else if (controller.MoveInput.magnitude > 0)
        {
            if (controller.IsCrouching)
            {
                stateMachine.ChangeState(stateMachine.CrouchState);
            }
            else if (controller.IsSprinting)
            {
                stateMachine.ChangeState(stateMachine.RunState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.WalkState);
            }
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {
        
    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        
    }
}
