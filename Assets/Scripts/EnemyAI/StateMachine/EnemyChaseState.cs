using UnityEngine;

public class EnemyChaseState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Chase State");
        stateMachine.EnemyAI.Agent.speed = stateMachine.EnemyAI.Data.chaseSpeed;
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        stateMachine.EnemyAI.Agent.SetDestination(stateMachine.EnemyAI.Target.transform.position);
    }

    public override void ExitState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Chase State");
        stateMachine.EnemyAI.Agent.ResetPath();
        stateMachine.EnemyAI.Agent.speed = stateMachine.EnemyAI.Data.baseSpeed;
    }
}
