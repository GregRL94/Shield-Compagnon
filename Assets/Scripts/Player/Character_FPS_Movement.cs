using System;
using UnityEngine;

[Serializable]
public class FPSMovementParameters
{
    [field: SerializeField, Tooltip("The speed at which the character moves.")] public float MoveSpeed { get; private set; } = 10f;
    [field: SerializeField, Tooltip("The speed at which the character rotates.")] public float RotationSpeed { get; private set; } = 5f;
    [field: SerializeField, Tooltip("Whether the character can move in the air.")] public bool AirControl { get; private set; } = false;

    [field: SerializeField, Tooltip("What is considered ground for the character.")] public LayerMask GroundLayer { get; private set; }
    [field: SerializeField, Tooltip("The half extents of the Box used for ground checking.")] public Vector3 GroundCheckBoxHalfExtents { get; private set; } = new Vector3(0.5f, 0.5f, 0.5f);
    [field: SerializeField, Tooltip("The distance for the sphere cast used for ground checking.")] public float GroundCheckDistance { get; private set; } = 0.5f;

    [field: SerializeField, Tooltip("The radius of the sphere used for ceiling checking.")] public float CeilingCheckRadius { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The distance for the sphere cast used for ceiling checking.")] public float CeilingCheckDistance { get; private set; } = 0.5f;

    [field: SerializeField, Tooltip("The speed multiplier when the character is crouching.")] public float CrouchSpeedMultiplier { get; private set; } = 0.5f;
    [field: SerializeField, Tooltip("The speed multiplier when the character is sprinting.")] public float SprintSpeedMultiplier { get; private set; } = 1.5f;
}


public class Character_FPS_Movement : MonoBehaviour
{
    #region Attributes
    public FPSMovementParameters movementParameters;

    private Rigidbody _rb;

    [Header("References")]
    [SerializeField] private GameObject FPSCam; // Anchor for the camera
    [SerializeField] private float _camMaxXAngle = 85f;
    private float _camXAngle;

    private bool _isGrounded;
    private bool _isCeilingFree;
    private bool _isMoving;
    #endregion Attributes

    #region Monobehaviour Flow
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        
    }
    #endregion Monobehaviour Flow

    #region Movement Checks
    void CheckIfGrounded()
    {
        bool groundDetected = Physics.BoxCast(transform.position, movementParameters.GroundCheckBoxHalfExtents, Vector3.down, Quaternion.identity, movementParameters.GroundCheckDistance, movementParameters.GroundLayer);
        _isGrounded = groundDetected && _rb.linearVelocity.y <= 0f; // Ensure the character is moving downwards or stationary to be considered grounded
    }

    void CheckCeiling()
    {
        _isCeilingFree = !Physics.SphereCast(transform.position, movementParameters.CeilingCheckRadius, Vector3.up, out RaycastHit hit, movementParameters.CeilingCheckDistance);
    }
    #endregion MovementChecks

    #region Movement Handlers
    void HandleRotation()
    {
        Vector2 lookDir = PlayerInputHandler.Instance.GetPLayerLook();
        _rb.angularVelocity = new Vector3(0f, lookDir.x * movementParameters.RotationSpeed, 0f);
        _camXAngle = Mathf.Clamp(_camXAngle - lookDir.y * movementParameters.RotationSpeed * Time.fixedDeltaTime, -_camMaxXAngle, _camMaxXAngle);
        FPSCam.transform.localEulerAngles = new Vector3(_camXAngle, 0f, 0f);
    }
    #endregion Movement Handlers
}
