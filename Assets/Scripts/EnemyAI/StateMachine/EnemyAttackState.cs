using UnityEngine;

public class EnemyAttackState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Attack State");
        var agent = stateMachine.EnemyAI.Agent;

        agent.ResetPath();
        agent.velocity = Vector3.zero;
        agent.isStopped = true;
        agent.updateRotation = false;
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        EnemyBaseAI enemyAI = stateMachine.EnemyAI;
        float distanceToTarget = Vector3.Distance(enemyAI.transform.position, enemyAI.Target.transform.position);

        if (enemyAI.isPlayerVisible && distanceToTarget <= enemyAI.Data.AttackRange)
        {
            enemyAI.LookAtTarget();
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
        stateMachine.EnemyAI.Agent.isStopped = false;
        stateMachine.EnemyAI.Agent.updateRotation = true;
    }
}
