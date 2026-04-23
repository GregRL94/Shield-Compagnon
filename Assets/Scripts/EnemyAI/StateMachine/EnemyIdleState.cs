using UnityEngine;

public class EnemyIdleState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Idle State");
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Updating Idle State...");
        // Here you would add logic to check for player proximity or other conditions to transition to another state
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Idle State");
    }
}
