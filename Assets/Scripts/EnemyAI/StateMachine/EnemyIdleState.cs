using UnityEngine;

public class EnemyIdleState: EnemyBaseState
{
    Vector3 _idlePoint;
    bool _idlePointSetup;

    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Idle State");
        Vector3 enemyAIPos = stateMachine.EnemyAI.transform.position;
        float tolerance = stateMachine.EnemyAI.Data.WaypointTolerance;

        // If there are no patrol waypoints defined, sets the current position as the idle point
        if (!_idlePointSetup)
        {
            _idlePoint = enemyAIPos;
            _idlePointSetup = true;
            Debug.Log("New idle point defined: " + _idlePoint);
        }

        // If not already at the idle point, sets the idle point as the destination for the NavMeshAgent
        if (Vector3.Distance(enemyAIPos, _idlePoint) > tolerance)
        {
            stateMachine.EnemyAI.Agent.SetDestination(_idlePoint);
        }
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        if (stateMachine.EnemyAI.isPlayerVisible)
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
        }
    }

    public override void ExitState(EnemyStateMachine stateMachine)
    {
		Debug.Log("Exiting Idle State");
        stateMachine.EnemyAI.Agent.ResetPath();
    }
}
