using UnityEngine;

public class EnemyAttackState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Attack State");
        stateMachine.EnemyAI.Agent.ResetPath();
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        EnemyBaseAI enemyAI = stateMachine.EnemyAI;
        float distanceToTarget = Vector3.Distance(enemyAI.transform.position, enemyAI.Target.transform.position);

        if (enemyAI.isPlayerVisible && distanceToTarget <= enemyAI.Data.AttackRange)
        {
            enemyAI.TryAttack();
        }
        else if (enemyAI.isPlayerVisible && distanceToTarget > enemyAI.Data.AttackRange)
        {
            stateMachine.ChangeState(stateMachine.ChaseState);
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
        Debug.Log("Exiting Attack State");
    }
}
