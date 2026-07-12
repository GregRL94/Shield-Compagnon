using UnityEngine;

public class PlayerRunState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {

    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (controller.MoveInput.magnitude == 0)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
        else if (!controller.IsSprinting)
        {
            stateMachine.ChangeState(stateMachine.WalkState);
        }
        else if (controller.IsCrouching)
        {
            stateMachine.ChangeState(stateMachine.CrouchState);
        }
        else if (controller.IsJumping)
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
