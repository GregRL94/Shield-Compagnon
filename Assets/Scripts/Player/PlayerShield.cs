using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Movement")]
    public Vector3 velocity = new Vector3(3f, 3f, 0f); // Initial direction + speed
    public float speed = 5f;

    [Header("Bounce")]
    public LayerMask _bounceableLayers; // Layers the object can bounce off

    [Header("Collision detetion")]
    [SerializeField] private int _nbOfRays;
    [SerializeField] private float _noColDetectAfterBounceDuration;
    private float _raycastDistance;
    private Bounds _bounds;
    bool _justBounced = false;
    private float _bounceTimer = 0f;

    private void Start()
    {
        _bounds = GetComponent<Collider>().bounds;
        _raycastDistance = _bounds.extents.x;
    }

    void Update()
    {
        Move();
    }

    void Move()
    {
        Vector3 moveDirection = velocity.normalized;
        float moveDistance = speed * Time.deltaTime;
        Vector3 previousVelocity = velocity;

        // Cast a ray in the direction of movement to detect surfaces ahead
        if (CollisionDetection(out RaycastHit hit) && !_justBounced)
        //if (Physics.Raycast(transform.position, moveDirection, out RaycastHit hit, moveDistance + _raycastDistance, _bounceableLayers))
        {
            // --- Law of Reflection ---
            // Reflect the velocity across the surface normal
            velocity = Vector3.Reflect(velocity, hit.normal);

            // Move to the hit point, then continue remaining movement in new direction
            float remainingDistance = moveDistance - hit.distance;
            transform.position = (hit.point - previousVelocity.normalized * _raycastDistance) + velocity.normalized * remainingDistance;
            _justBounced = true;
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
            if (Vector3.Dot(velocity.normalized, rayDirection) > 0.5f)
            {
                if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, _raycastDistance, _bounceableLayers))
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
}
