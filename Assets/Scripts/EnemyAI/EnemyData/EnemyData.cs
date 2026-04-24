using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    public float health;
    public float baseSpeed;
    public float patrolSpeed;
    public float chaseSpeed;
    public float attackRange;
    public float attackSpeed;
    public float damage;
    public float detectionRadius;
    public float visionAngle;
    public LayerMask visionObstacleMask;
    public Transform[] waypoints;
    public float waypointTolerance;
}
