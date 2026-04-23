using UnityEngine;

public class EnemyStateMachine
{

    EnemyBaseState currentState;
    EnemyIdleState idleState;
    EnemyPatrolState patrolState;
    EnemyChaseState chaseState;
    EnemyAttackState attackState;

    public EnemyStateMachine(EnemyBaseState startingState)
    {
        Initialize(startingState);
    }

    void Initialize(EnemyBaseState startingState)
    {
        idleState = new EnemyIdleState();
        patrolState = new EnemyPatrolState();
        chaseState = new EnemyChaseState();
        attackState = new EnemyAttackState();

        currentState = startingState;
        currentState.EnterState(this);
    }

    // Update is called once per frame
    void Update()
    {
        currentState.UpdateState(this);
    }
}
