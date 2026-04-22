using UnityEngine;

public class EnemyStateMachine : MonoBehaviour
{
    EnemyBaseState currentState;
    EnemyPatrolState patrolState = new EnemyPatrolState();
    EnemyChaseState chaseState = new EnemyChaseState();
    EnemyAttackState attackState = new EnemyAttackState();

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
