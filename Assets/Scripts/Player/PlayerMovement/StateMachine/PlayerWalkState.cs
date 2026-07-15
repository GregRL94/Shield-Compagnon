using UnityEngine;

public class PlayerWalkState : PlayerMovementBaseState
{
    float _moveSpeed;

    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        _controller = stateMachine.Controller;
        _moveSpeed = _controller.MovementParams.BaseMoveSpeed;
        Debug.Log("Entered Walk State");
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        if (stateMachine.Controller.MoveInput.magnitude == 0)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
            return;
        }
        if (stateMachine.Controller.IsSprinting)
        {
            stateMachine.ChangeState(stateMachine.RunState);
            return;
        }
        if (stateMachine.Controller.IsCrouching)
        {
            stateMachine.ChangeState(stateMachine.CrouchState);
            return;
        }
        if (stateMachine.Controller.IsJumping)
        {
            stateMachine.ChangeState(stateMachine.JumpState); // Currently broken, will fix later
            return;
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {
        _controller.HandleRotation(_controller.MovementParams.BaseRotationSpeed);
        _controller.HandleMovement(_moveSpeed);
    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        Debug.Log("Exited Walk State");
    }
}
