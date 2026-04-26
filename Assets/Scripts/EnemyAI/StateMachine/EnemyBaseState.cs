using UnityEngine;

public abstract class EnemyBaseState
{
    public abstract void EnterState(EnemyStateMachine stateMachine);
    public abstract void UpdateState(EnemyStateMachine stateMachine);
    public abstract void ExitState(EnemyStateMachine stateMachine);
}
