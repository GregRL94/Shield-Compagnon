using UnityEngine;

public class PlayerCrouchState : PlayerMovementBaseState
{
    float _moveSpeed;

    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        _moveSpeed = controller.MovementParams.BaseMoveSpeed * controller.MovementParams.CrouchSpeedMultiplier;
        controller.IsCrouching = true;
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (!controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }

        if (!controller.PlayerInputs.CrouchPressed() && controller.IsCeilingFree)
        {
            if (controller.MoveInput.magnitude <= 0.1f)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
                return;
            }
            if (controller.PlayerInputs.SprintPressed())
            {
                stateMachine.ChangeState(stateMachine.RunState);
                return;
            }
            stateMachine.ChangeState(stateMachine.WalkState);
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        stateMachine.Controller.HandleRotation(controller.MovementParams.BaseRotationSpeed);
        stateMachine.Controller.HandleMovement(_moveSpeed);
    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        stateMachine.Controller.IsCrouching = false;
    }
}
