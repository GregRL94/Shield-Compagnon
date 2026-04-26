using UnityEngine;

public class EnemyMeleeAI : EnemyBaseAI
{
    [SerializeField] private Transform _attackPoint;

    protected override void Attack()
    {
        Debug.Log("Attack from Melee AI !");
        foreach (Collider hit in Physics.OverlapSphere(_attackPoint.position, Data.AttackRange, Data.AttacksAffectWhat))
        {
            if (hit.TryGetComponent<IHittable>(out IHittable hittable))
            {
                hittable.TakeHit(Data.Damage);
            }
        }
    }

    protected override void CreateStateMachine()
    {
        _stateMachine = new EnemyStateMachine(this);
    }
}
