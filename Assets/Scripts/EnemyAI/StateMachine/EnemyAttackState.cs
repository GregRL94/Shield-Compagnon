using UnityEngine;

public class EnemyAttackState: EnemyBaseState
{
    public override void EnterState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Entering Attack State");
    }

    public override void UpdateState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Updating Attack State...");
        // Here you would add logic to perform the attack, such as reducing the player's health
    }

    public override void ExitState(EnemyStateMachine stateMachine)
    {
        Debug.Log("Exiting Attack State");
    }

    private void AttackTarget()
    {
        Debug.Log("Attacking the target!");
        // Implement attack logic here, such as applying damage to the player
    }
}
