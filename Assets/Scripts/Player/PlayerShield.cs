using System.Collections.Generic;
using UnityEngine;

public class PlayerShield : MonoBehaviour
{
    #region Attributes
    [Header("Movement")]
    public float speed = 10f;

    [Header("Bounce & Stick")]
    [field: SerializeField, Tooltip("Layers the shield can bounce off")] private LayerMask _bounceableLayers;
    [field: SerializeField, Tooltip("Layers the shield can stick to")] private LayerMask _stickableLayers;
    [field: SerializeField, Tooltip("Depth the shield will stick into the surface")] private float _stickDepth;

    [Header("Collision detection")]
    [field: SerializeField, Tooltip("The number of rays used to detect collisions. The more rays, the more accurate, at the cost of performance")] private int _nbOfRays = 16;
    [field: SerializeField, Tooltip("Minimum dot product to consider a ray valid for bouncing")] private float _minRayAlignement = 0.25f;
    [field: SerializeField, Tooltip("Duration after a bounce during which collisions are ignored")] private float _noColAfterBounceDuration = 0.15f;

    LayerMask _originalLayer;
    Bounds _bounds;
    Vector3 _moveDirection;
    float _raycastDistance;
    float _bounceTimer;
    bool _justBounced;
    bool _isStuck;
    bool _isOnCharacter;
    #endregion Attributes

    #region MonoBehaviour Flow
    void Start()
    {
        _bounds = GetComponent<Collider>().bounds;
        _raycastDistance = _bounds.extents.x;
        _originalLayer = gameObject.layer;
    }

    void Update()
    {
        UpdateTimers();
        if (!_isStuck && !_isOnCharacter) { HandleCollsions(); }
        Move();
    }
    #endregion MonoBehaviour Flow

    #region Public Methods
    public void OnShieldThrown(Vector3 moveDirection)
    {
        _moveDirection = moveDirection;
    }

    public void OnShieldCalled(Vector3 targetPoint)
    { 
        _moveDirection = targetPoint - transform.position;
    }

    public void OnShieldAttached(bool isAttached)
    {
        if (isAttached && _isStuck) { _isStuck = false; }
        _moveDirection = Vector3.zero;
        _isOnCharacter = isAttached;
    }
    #endregion Public Methods

    #region Private Methods
    void Move()
    {
        if (_moveDirection == Vector3.zero) { return; }
        transform.position += _moveDirection.normalized * speed * Time.deltaTime;
    }

    void UpdateTimers()
    {
        if (_bounceTimer >= 0) { _bounceTimer -= Time.deltaTime; }
        else if (_justBounced) { _justBounced = false; }
    }
    #endregion Private Methods

    #region Manual Collisions Handling
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
    #endregion Manual Collisions Handling

    #region OnTriggers
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player")) { return; }
        if (other.TryGetComponent<IHittable>(out IHittable hittable))
        {
            if (Vector3.Distance(transform.position, other.transform.position) < _bounds.extents.x)
            {
                hittable.TakeHit(20f);
            }
        }
    }

    void OnTriggerExit(Collider other)
    {
        if (_isStuck)
        {
            _isStuck = false;
            gameObject.layer = _originalLayer;
        }
    }
    #endregion OnTriggers

    #region Subscriptions
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
            CharacterActions.ShieldAttach += OnShieldAttached;
            CharacterActions.ShieldThrow += OnShieldThrown;
        }
        else
        {
            CharacterActions.ShieldAttach -= OnShieldAttached;
            CharacterActions.ShieldThrow -= OnShieldThrown;
        }
    }
    #endregion Subscriptions
}
