using UnityEngine;
using UnityEngine.AI;

public class EnemyBaseAI : MonoBehaviour, IHittable
{
    [field: SerializeField, Tooltip("The enemy configuration data.")] public EnemyData Data { get; protected set; }
    [field: SerializeField, Tooltip("The patrol waypoints for the enemy.")] public Transform[] Waypoints { get; protected set; }
    [field: SerializeField, Tooltip("The tolerance for reaching waypoints.")] public float WaypointTolerance { get; private set; }

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
    protected float _nextAttackTimer;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    protected virtual void Start()
    {
        if (TryGetComponent<NavMeshAgent>(out NavMeshAgent agent))
        {
            Agent = agent;
            Agent.speed = Data.BaseSpeed;
            Agent.angularSpeed = Data.RotationSpeed;
            Agent.stoppingDistance = Data.AttackRange;
        }
        else
        {
            Debug.LogError("NavMeshAgent component is missing on " + gameObject.name);
        }
        CurrentHealth = Data.Health;
        CreateDetectionCollider();
        CreateStateMachine();
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        UpdateTimers();
        if (!isDead)
        {
            LineOfSight();
        }
        _stateMachine.UpdateStateMachine();
    }

    protected virtual void UpdateTimers()
    {
        if (_nextAttackTimer > 0f) { _nextAttackTimer -= Time.deltaTime; }
    }

    protected void LineOfSight()
    {
        if (Target == null)
        {
            isPlayerVisible = false;
            return;
        }

        Vector3 dir = Target.transform.position - transform.position;
        float angle = Vector3.Angle(transform.forward, dir); // Angle is unsigned and always between 0 and 180 degrees (Unity Documentation)

        // Check if the player is within the vision angle
        if (angle <= Data.VisionAngle / 2)
        {
            // Check for obstacles between the enemy and the player
            RaycastHit hit;
            if (!Physics.Raycast(transform.position, dir.normalized, out hit, Vector3.Distance(transform.position, Target.transform.position), Data.VisionObstacleMask))
            {
                if (!isPlayerVisible) { Debug.Log("Player detected by " + gameObject.name + "!"); }
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

    public void LookAtTarget()
    {
        if (Target != null && isPlayerVisible)
        {
            //transform.LookAt(Target.transform.position);
            Vector3 direction = (Target.transform.position - transform.position).normalized;
            Quaternion lookRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * (Data.RotationSpeed / 10f));
        }
    }

    public virtual bool TryAttack()
    {
        if (_nextAttackTimer <= 0f)
        {
            Attack();
            _nextAttackTimer = 1 / Data.AttackSpeed;
            return true;
        }
        return false;
    }

    protected virtual void Attack()
    {
        // Derived classes should override this method.
        Debug.Log("Attacking the target!");
    }

    protected void TakeDamage(float damage)
    {
        if (isDead) { return; }
        CurrentHealth -= damage;
        CurrentHealth = Mathf.Clamp(CurrentHealth, 0f, Data.Health);
        Debug.Log(gameObject.name + " took " + damage + " damage. Current health: " + CurrentHealth);
        if (CurrentHealth <= 0f)
        {
            Die();
        }
    }

    protected virtual void Die()
    {
        isDead = true;
        Agent.isStopped = true;
        Debug.Log(gameObject.name + " has died.");
    }

    protected virtual void CreateStateMachine()
    {
        // Derived classes should override this method.
        _stateMachine = new EnemyStateMachine(this);
    }

    protected virtual void CreateDetectionCollider()
    {
        _detectionCollider = gameObject.AddComponent<SphereCollider>();
        _detectionCollider.isTrigger = true;
        ((SphereCollider)_detectionCollider).radius = Data.DetectionRadius;
    }

    protected virtual void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player entered detection range of " + gameObject.name);
            Target = other.gameObject;
        }
    }

    protected virtual void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Player exited detection range of " + gameObject.name);
            Target = null;
        }
    }

    public void TakeHit(float value)
    {
        TakeDamage(value);
    }
}
