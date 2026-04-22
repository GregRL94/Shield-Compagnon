using UnityEngine;

public class EnemyChaseState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Chase State");
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Updating Chase State...");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Chase State");
    }

    private void MoveTowardsTarget()
    {
        Debug.Log("Moving towards the target...");
        // Implement movement logic here, such as using Unity's NavMeshAgent to move towards the player
    }
}
