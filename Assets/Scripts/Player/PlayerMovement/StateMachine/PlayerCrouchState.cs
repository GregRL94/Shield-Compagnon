using UnityEngine;

public class PlayerCrouchState : PlayerMovementBaseState
{
    public override void EnterState(PlayerMovementStateMachine stateMachine)
    {

    }

    public override void UpdateState(PlayerMovementStateMachine stateMachine)
    {
        var controller = stateMachine.Controller;

        if (!controller.IsCrouching)
        {
            if (controller.MoveInput.magnitude == 0)
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
            else
            {
                if (controller.IsSprinting)
                {
                    stateMachine.ChangeState(stateMachine.RunState);
                }
                else
                {
                    stateMachine.ChangeState(stateMachine.WalkState);
                }
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
