using UnityEngine;

public class PlayerJumpState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;
        controller.IsJumping = true;
        controller.Jump();
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (!controller.IsJumping)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {

    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;
        controller.IsJumping = false;
    }
}
