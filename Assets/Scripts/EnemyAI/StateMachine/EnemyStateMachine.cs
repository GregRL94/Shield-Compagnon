using UnityEngine;

public class EnemyStateMachine
{
    public EnemyBaseAI EnemyAI { get; private set; }

    public EnemyIdleState IdleState { get; private set; }
    public EnemyPatrolState PatrolState { get; private set; }
    public EnemyChaseState ChaseState { get; private set; }
    public EnemyAttackState AttackState { get; private set; }
    EnemyBaseState _currentState;

    public EnemyStateMachine(EnemyBaseAI enemyAI)
    {
        EnemyAI = enemyAI;
        Initialize();
    }

    void Initialize()
    {
        IdleState = new EnemyIdleState();
        PatrolState = new EnemyPatrolState();
        ChaseState = new EnemyChaseState();
        AttackState = new EnemyAttackState();

        _currentState = EnemyAI.Waypoints.Length > 0 ? PatrolState : IdleState;
        _currentState.EnterState(this);
    }

    public void UpdateStateMachine()
    {
        _currentState.UpdateState(this);
    }

    public void ChangeState(EnemyBaseState newState)
    {
        _currentState.ExitState(this);
        _currentState = newState;
        _currentState.EnterState(this);
    }
}
