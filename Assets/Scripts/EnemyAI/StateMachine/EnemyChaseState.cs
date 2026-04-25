using UnityEngine;

public class EnemyChaseState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Chase State");
        stateMachine.EnemyAI.Agent.speed = stateMachine.EnemyAI.Data.ChaseSpeed;
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        EnemyBaseAI enemyAI = stateMachine.EnemyAI;

        if (enemyAI.isPlayerVisible)
        {
            if (Vector3.Distance(enemyAI.transform.position, enemyAI.Target.transform.position) <= enemyAI.Data.AttackRange)
            {
                stateMachine.ChangeState(stateMachine.AttackState);
            }
            else
            {
                enemyAI.Agent.SetDestination(enemyAI.Target.transform.position);
            }
        }
        else
        {
            if (enemyAI.Waypoints.Length > 0)
            {
                stateMachine.ChangeState(stateMachine.PatrolState);
            }
            else
            {
                stateMachine.ChangeState(stateMachine.IdleState);
            }
        }
    }

    public override void ExitState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Chase State");
        stateMachine.EnemyAI.Agent.ResetPath();
        stateMachine.EnemyAI.Agent.speed = stateMachine.EnemyAI.Data.BaseSpeed;
    }
}
