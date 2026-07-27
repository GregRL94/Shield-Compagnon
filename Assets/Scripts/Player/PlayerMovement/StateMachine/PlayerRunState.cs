using UnityEngine;

public class PlayerRunState : PlayerMovementBaseState
{
    float _moveSpeed;

    // public PlayerRunState(PlayerMovementController controller) : base(controller) { }

    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        Debug.Log("Entered Run State");
        var movementParams = stateMachine.Controller.MovementParams;

        _moveSpeed = movementParams.BaseMoveSpeed * movementParams.SprintSpeedMultiplier;
        stateMachine.Controller.IsSprinting = true;
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (!controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }

        if (controller.MoveInput.magnitude <= 0.1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        if (!controller.PlayerInputs.SprintPressed())
        {
            if (controller.PlayerInputs.CrouchPressed())
            {
                stateMachine.ChangeState(stateMachine.CrouchState);
                return;
            }
            stateMachine.ChangeState(stateMachine.WalkState);
            return;
        }

        if (controller.PlayerInputs.JumpPressed())
        {
            stateMachine.ChangeState(stateMachine.JumpState);
            return;
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        controller.HandleRotation(controller.MovementParams.BaseRotationSpeed);
        controller.HandleMovement(_moveSpeed);
    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        Debug.Log("Exited Run State");
        stateMachine.Controller.IsSprinting = false;
    }
}
