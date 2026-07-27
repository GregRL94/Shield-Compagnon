using UnityEngine;

public class PlayerWalkState : PlayerMovementBaseState
{
    float _moveSpeed;

    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;
        _moveSpeed = controller.MovementParams.BaseMoveSpeed;
        controller.IsMoving = true;
        Debug.Log("Entered Walk State");
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (!controller.IsGrounded)
        {
            stateMachine.ChangeState(stateMachine.FallState);
            return;
        }
        
        if (controller.PlayerInputs.SprintPressed())
        {
            stateMachine.ChangeState(stateMachine.RunState);
            return;
        }

        if (controller.MoveInput.magnitude <= 0.1f)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }

        if (controller.PlayerInputs.CrouchPressed())
        {
            stateMachine.ChangeState(stateMachine.CrouchState);
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
        Debug.Log("Exited Walk State");
    }
}
