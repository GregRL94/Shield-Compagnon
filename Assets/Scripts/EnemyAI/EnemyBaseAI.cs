using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseAI : MonoBehaviour
{
    [field: SerializeField, Tooltip("The enemy configuration data.")] public EnemyData Data { get; protected set; } // enemy configuration data
    [field: SerializeField, Tooltip("The point from which attacks are made.")] public Transform AttackPoint { get; protected set; } // The point from which attacks are made

    public NavMeshAgent Agent { get; protected set; }
    public GameObject Target { get; protected set; }
    protected EnemyStateMachine _stateMachine;
    protected Collider _detectionCollider;

    public bool isAttacking { get; protected set; }
    public bool isMoving { get; protected set; }
    public bool isDead { get; protected set; }
    public bool isPlayerVisible { get; protected set; }
    public Vector3 LastKnownPlayerPosition { get; protected set; }
    public float CurrentHealth { get; protected set; }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            Agent = agent;
            Agent.speed = Data.baseSpeed;
            Agent.stoppingDistance = Data.attackRange - 0.1f * Data.attackRange; // Stop slightly after the attack range to ensure it can attack
        }
        else
        {
            Debug.LogError("NavMeshAgent component is missing on " + gameObject.name);
        }
        CurrentHealth = Data.health;
        CreateDetectionCollider();
        CreateStateMachine();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!isDead && Target != null)
        {
            LineOfSight();
        }
        _stateMachine.UpdateStateMachine();
    }

    protected virtual void LineOfSight()
    {
        Vector3 dir = Target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir); // Angle is unsigned and always between 0 and 180 degrees (Unity Documentation)

        // Check if the player is within the vision angle
        if (angle <= Data.visionAngle / 2)
        {
            // Check for obstacles between the enemy and the player
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, dir.normalized, out hit, Vector3.Distance(transform.position, Target.transform.position), Data.visionObstacleMask))
            {
                isPlayerVisible = true;
                LastKnownPlayerPosition = Target.transform.position;
            }
            else
            {
                isPlayerVisible = false;
            }
        }
        else
        {
            isPlayerVisible = false;
        }
    }

    protected virtual void CreateStateMachine()
    {
        _stateMachine = new EnemyStateMachine(this);
    }

    protected virtual void CreateDetectionCollider()
    {
        _detectionCollider = gameObject.AddComponent<SphereCollider>();
        _detectionCollider.isTrigger = true;
        ((SphereCollider)_detectionCollider).radius = Data.detectionRadius;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Target = other.gameObject;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Target = null;
        }
    }
}
