using UnityEngine;

public class EnemyPatrolState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Patrol State");
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Updating Patrol State...");
    }

    public override void ExitState()
    {
        Debug.Log("Exiting Patrol State");
    }

    private void MoveAlongPatrolPath()
    {
        Debug.Log("Moving along the patrol path...");
        // Implement patrol movement logic here, such as moving between waypoints
    }
}
