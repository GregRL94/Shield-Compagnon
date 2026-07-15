using UnityEngine;

public class PlayerJumpState : PlayerMovementBaseState
{
    bool _hasJumped;
    // public PlayerJumpState(PlayerMovementController controller) : base(controller) { }

    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {
        _controller = stateMachine.Controller;
        _controller.IsJumping = true;
        _controller.HandleJump(Vector3.zero); // To modify with actual values
    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        if (!_controller.IsJumping)
        {
            stateMachine.ChangeState(stateMachine.IdleState);
        }
    }

    public override void FixedUpdateState(PlayerMovementStateMachine stateMachine)
    {
        if (!_hasJumped)
        {
            _controller.HandleJump(_controller.transform.up * _controller.JumpParams.JumpForce);
            _hasJumped = true;
        }
    }

    public override void ExitState(PlayerMovementStateMachine stateMachine)
    {
        _controller.IsJumping = false;
    }
}
