using UnityEngine;

public class PlayerIdleState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        Debug.Log("Entered Idle State");
        stateMachine.Controller.IsMoving = false;
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (!controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }

        if (controller.PlayerInputs.CrouchPressed())
        {
            stateMachine.ChangeState(stateMachine.CrouchState);
            return;
        }

        if (controller.PlayerInputs.SprintPressed() && controller.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.RunState);
        }
        else if (controller.MoveInput.magnitude > 0.1f)
        {
            stateMachine.ChangeState(stateMachine.WalkState);
        }

        if (controller.PlayerInputs.JumpPressed())
        {
            stateMachine.ChangeState(stateMachine.JumpState);
            return;
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine) { }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        Debug.Log("Exited Idle State");
    }
}
