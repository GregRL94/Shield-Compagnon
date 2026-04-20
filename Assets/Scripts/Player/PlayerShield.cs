using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 velocity = new Vector3(3f, 3f, 0f); // Initial direction + speed
    public float speed = 5f;

    [Header("Bounce")]
    public LayerMask _bounceableLayers; // Layers the object can bounce off
    public LayerMask _stickableLayers; // Layers the object can stick to (not implemented yet)

    [Header("Collision detection")]
    [SerializeField] private int _nbOfRays = 16;
    [SerializeField, Range(0f, 1f)] private float _minRayAlignement = 0.25f; // Minimum dot product to consider a ray valid for bouncing
    [SerializeField] private float _noColAfterBounceDuration = 0.15f;
    [SerializeField] private float _stickDepth;
    private float _raycastDistance;
    private Bounds _bounds;
    bool _justBounced = false;
    bool _isStuck = false;
    private float _bounceTimer = 0f;

    private void Start()
    {
        _bounds = GetComponent<Collider>().bounds;
        _raycastDistance = _bounds.extents.x;
    }

    void Update()
    {
        UpdateTimers();
        Move();
    }

    void Move()
    {
        Vector3 moveDirection = velocity.normalized;
        float moveDistance = speed * Time.deltaTime;
        Vector3 previousVelocity = velocity;

        if (_isStuck) { return; }
        if (CollisionDetection(out RaycastHit hit) && !_justBounced)
        {
            // --- Law of Reflection ---
            // Reflect the velocity across the surface normal
            if (((1 << hit.collider.gameObject.layer) & _bounceableLayers) != 0)
            {
                Debug.Log($"Bounced on {hit.collider.gameObject.name}");
                velocity = Vector3.Reflect(velocity, hit.normal);
                _bounceTimer = _noColAfterBounceDuration;
                _justBounced = true;
            }
            else if (((1 << hit.collider.gameObject.layer) & _stickableLayers) != 0)
            {
                Debug.Log($"Stuck to {hit.collider.gameObject.name}");
                transform.position += -hit.normal * _stickDepth; // Move slightly into the surface to "stick"
                velocity = Vector3.zero;
                _isStuck = true;
            }
        }
        else
        {
            // No surface ahead — move normally
            transform.position += moveDirection * moveDistance;
        }
    }

    bool CollisionDetection(out RaycastHit farthestHit)
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        farthestHit = new RaycastHit();
        float farthestDistance = 0f;

        for (int i = 0; i < _nbOfRays; i++)
        {
            float angle = (360f / _nbOfRays) * i;
            Vector3 rayDirection = Quaternion.Euler(0, angle, 0) * Vector3.forward;
            if (Vector3.Dot(velocity.normalized, rayDirection) > _minRayAlignement)
            {
                if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, _raycastDistance))
                {
                    hits.Add(hit);
                    Debug.DrawRay(transform.position, rayDirection * hit.distance, Color.red);
                }
                else
                {
                    Debug.DrawRay(transform.position, rayDirection * _raycastDistance, Color.green);
                }
            }
            else
            {
                Debug.DrawRay(transform.position, rayDirection * _raycastDistance, Color.black);
            }
        }

        if (hits.Count <= 0)
        {
            return false;
        }

        foreach (RaycastHit hit in hits)
        {
            float distance = Vector3.Distance(transform.position, hit.point);
            if (distance > farthestDistance)
            {
                farthestDistance = distance;
                farthestHit = hit;
            }
        }
        return true;
    }

    void UpdateTimers()
    {
        if (_bounceTimer >= 0) { _bounceTimer -= Time.deltaTime; }
        else if (_justBounced) { _justBounced = false; }
    }
}
