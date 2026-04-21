using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;

    [Header("Bounce")]
    public LayerMask _bounceableLayers; // Layers the object can bounce off
    public LayerMask _stickableLayers; // Layers the object can stick to (not implemented yet)

    [Header("Collision detection")]
    [SerializeField] private int _nbOfRays = 16;
    [SerializeField, Range(0f, 1f)] private float _minRayAlignement = 0.25f; // Minimum dot product to consider a ray valid for bouncing
    [SerializeField] private float _noColAfterBounceDuration = 0.15f;
    [SerializeField] private float _stickDepth;
    private LayerMask _originalLayer;
    private float _raycastDistance;
    private Bounds _bounds;
    bool _justBounced;
    bool _isStuck;
    private float _bounceTimer;
    private Vector3 _moveDirection;

    private void Start()
    {
        _bounds = GetComponent<Collider>().bounds;
        _raycastDistance = _bounds.extents.x;
        _originalLayer = gameObject.layer;
    }

    void Update()
    {
        UpdateTimers();
        if (!_isStuck) { HandleCollsions(); }
        Move();
    }

    public void SetMoveDirection(Vector3 moveDirection)
    {
        _moveDirection = moveDirection;
    }

    public void SetTargetPoint(Vector3 targetPoint)
    { 
        _moveDirection = targetPoint - transform.position;
    }

    void Move()
    {
        if (_moveDirection == Vector3.zero) { return; }
        transform.position += _moveDirection.normalized * speed * Time.deltaTime;
    }

    void HandleCollsions()
    {
        if (CollisionDetection(out RaycastHit hit) && !_justBounced)
        {
            // --- Law of Reflection ---
            // Reflect the velocity across the surface normal
            if (((1 << hit.collider.gameObject.layer) & _bounceableLayers) != 0)
            {
                _moveDirection = Vector3.Reflect(_moveDirection, hit.normal);
                _bounceTimer = _noColAfterBounceDuration;
                _justBounced = true;
            }
            else if (((1 << hit.collider.gameObject.layer) & _stickableLayers) != 0)
            {
                transform.position += -hit.normal * _stickDepth; // Move slightly into the surface to "stick"
                gameObject.layer = LayerMask.NameToLayer("Ground");
                _moveDirection = Vector3.zero;
                _isStuck = true;
            }
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
            if (Vector3.Dot(_moveDirection.normalized, rayDirection) > _minRayAlignement)
            {
                if (Physics.Raycast(transform.position, rayDirection, out RaycastHit hit, _raycastDistance))
                {
                    hits.Add(hit);
                }
            }
        }

        if (hits.Count <= 0) { return false; }

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

    void OnTriggerExit(Collider other)
    {
        if (_isStuck)
        {
            _isStuck = false;
            gameObject.layer = _originalLayer;
        }
    }

    private void OnEnable()
    {
        Subscribe(true);
    }

    private void OnDisable()
    {
        Subscribe(false);
    }

    private void OnDestroy()
    {
        Subscribe(false);
    }

    private void Subscribe(bool isSubscribed)
    {
        if (isSubscribed)
        {
            CharacterActions.ThrowShield += SetMoveDirection;
            CharacterActions.CallShield += SetTargetPoint;
        }
        else
        {
            CharacterActions.ThrowShield -= SetMoveDirection;
            CharacterActions.CallShield -= SetTargetPoint;
        }
    }
}
