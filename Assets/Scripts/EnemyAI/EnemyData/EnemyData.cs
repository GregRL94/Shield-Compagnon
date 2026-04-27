using UnityEngine;

[CreateAssetMenu(fileName = "EnemyData", menuName = "Scriptable Objects/Enemy/EnemyData")]
public class EnemyData : ScriptableObject
{
    [field: SerializeField, Tooltip("The health of the enemy.")] public float Health { get; private set; }
    [field: SerializeField, Tooltip("The rotation speed of the enemy.")] public float RotationSpeed { get; private set; }
    [field: SerializeField, Tooltip("The base speed of the enemy.")] public float BaseSpeed { get; private set; }
    [field: SerializeField, Tooltip("The patrol speed of the enemy.")] public float PatrolSpeed { get; private set; }
    [field: SerializeField, Tooltip("The chase speed of the enemy.")] public float ChaseSpeed { get; private set; }
    [field: SerializeField, Tooltip("The attack range of the enemy.")] public float AttackRange { get; private set; }
    [field: SerializeField, Tooltip("The attack speed of the enemy.")] public float AttackSpeed { get; private set; }
    [field: SerializeField, Tooltip("The layers that attacks can affect.")] public LayerMask AttacksAffectWhat { get; private set; }
    [field: SerializeField, Tooltip("The damage dealt by the enemy.")] public float Damage { get; private set; }
    [field: SerializeField, Tooltip("The detection radius of the enemy.")] public float DetectionRadius { get; private set; }
    [field: SerializeField, Tooltip("The vision angle of the enemy.")] public float VisionAngle { get; private set; }
    [field: SerializeField, Tooltip("The layer mask for vision obstacles.")] public LayerMask VisionObstacleMask { get; private set; }
}
