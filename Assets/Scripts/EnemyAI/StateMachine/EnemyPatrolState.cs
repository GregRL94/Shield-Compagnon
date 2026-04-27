using UnityEngine;

public class EnemyPatrolState: EnemyBaseState
{
    int _lastWaypointIndex;
    int _currentWaypointIndex;

    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Patrol State");
        var agent = stateMachine.EnemyAI.Agent;

        agent.speed = stateMachine.EnemyAI.Data.PatrolSpeed;
        _currentWaypointIndex = _lastWaypointIndex;
        agent.SetDestination(stateMachine.EnemyAI.Waypoints[_currentWaypointIndex].position);
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        MoveAlongPatrolPath(stateMachine);
        if (stateMachine.EnemyAI.isPlayerVisible)
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
        }
    }

    public override void ExitState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Patrol State");
        var agent = stateMachine.EnemyAI.Agent;

        agent.ResetPath();
        agent.speed = stateMachine.EnemyAI.Data.BaseSpeed;
    }

    private void MoveAlongPatrolPath(EnemyStateMachine stateMachine)
    {
        Vector3 enemyAIPos = stateMachine.EnemyAI.transform.position;
        Vector3 currentWaypointPos = stateMachine.EnemyAI.Waypoints[_currentWaypointIndex].position;
        float tolerance = stateMachine.EnemyAI.WaypointTolerance;
        var agent = stateMachine.EnemyAI.Agent;

        // Check if the current waypoint has been reached
        if (Vector3.Distance(enemyAIPos, currentWaypointPos) <= tolerance)
        {
            Debug.Log("Waypoint " + _currentWaypointIndex + " reached.");
            _lastWaypointIndex = _currentWaypointIndex; // Update the last waypoint index to the current one
            _currentWaypointIndex++;
            if (_currentWaypointIndex >= stateMachine.EnemyAI.Waypoints.Length)
            {
                _currentWaypointIndex = 0;
            }
            agent.SetDestination(stateMachine.EnemyAI.Waypoints[_currentWaypointIndex].position);
        }
    }
}
