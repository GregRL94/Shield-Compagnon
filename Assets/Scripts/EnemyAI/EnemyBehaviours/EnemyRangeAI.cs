using UnityEngine;

public class EnemyRangeAI : EnemyBaseAI
{
    [SerializeField] private GameObject _projectilePrefab;
    [SerializeField] private Transform[] _firepoints;

    protected override void Attack()
    {
        Debug.Log("Attack from Range AI !");
        for (int i = 0; i < _firepoints.Length; i++)
        {
            Instantiate(_projectilePrefab, _firepoints[i].position, _firepoints[i].rotation);
        }
    }

    protected override void CreateStateMachine()
    {
        _stateMachine = new EnemyStateMachine(this);
    }
}
